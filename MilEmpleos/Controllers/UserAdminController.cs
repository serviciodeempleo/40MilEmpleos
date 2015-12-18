using IdentitySample.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MilEmpleos.Models;
using Newtonsoft.Json;

namespace IdentitySample.Controllers
{
    [Authorize(Roles = "Admin, Centros, Unidad")]
    public class UsersAdminController : Controller
    {
        private MilEmpleosEntities db = new MilEmpleosEntities();
        public UsersAdminController()
        {
        }

        public UsersAdminController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }
        //
        // GET: /Users/
        public ActionResult ListaUsers()
        {
            var USER_id = User.Identity.GetUserId();
            var ChangePassword = (from cp in db.AspNetUsers where cp.Id == USER_id select cp.PasswordChange).First();
            if (ChangePassword==true)
            {
                return RedirectToAction("ChangePassword", "Manage");
            }
            var CE = (from item in db.AspNetUsers
                      where item.Id == USER_id
                      select item.CentroId).First();
            var usuarioConsulta = (from u in db.AspNetUsers 
                                  where u.Id != USER_id && u.CentroId.Equals(CE)
                                   select u).OrderByDescending(x => x.Activo);
            return View(usuarioConsulta);
        }

        //
        // GET: /Users/Detalle/5
        public async Task<ActionResult> Detalle(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);

            ViewBag.RoleNames = await UserManager.GetRolesAsync(user.Id);

            return View(user);
        }
        // GET: /Users/Crear
        public async Task<ActionResult> Crear()
        {
            //Get the list of Roles
            ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
            return View();
        }

        //
        // POST: /Users/Crear
        [HttpPost]
        public async Task<ActionResult> Crear(RegisterViewModel userViewModel, params string[] selectedRoles)
        {
            LogsAudit logsAudit = new LogsAudit();
            logsAudit.Id = Guid.NewGuid();
            logsAudit.InstanciaServidor = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            logsAudit.UserName = User.Identity.Name;
            logsAudit.FechaHora = DateTime.Now;
            logsAudit.IpRequest = HttpContext.Request.UserHostAddress;
            logsAudit.DataRequest = HttpContext.Request.AppRelativeCurrentExecutionFilePath + " | " + HttpContext.Request.AnonymousID + " | " + HttpContext.Request.CurrentExecutionFilePathExtension + " | " + HttpContext.Request.UrlReferrer;
            logsAudit.Agent = HttpContext.Request.UserAgent;
            logsAudit.JsonRequest = JsonConvert.SerializeObject(userViewModel) + "||" + JsonConvert.SerializeObject(selectedRoles);
            logsAudit.Operacion = "Creación de usuario de consulta";
            string resultado = "El modelo  no es valido";

            if (ModelState.IsValid || userViewModel.CentroEmpleo==null)
            {
                var user = new ApplicationUser
                {
                    UserName = userViewModel.Email,
                    Email =
                        userViewModel.Email,
                    // Add the Address Info:
                    Address = userViewModel.Address,
                    City = userViewModel.City,
                    State = userViewModel.State,
                    PostalCode = userViewModel.PostalCode
                };
                var USER_id = User.Identity.GetUserId();
                var CE = (from item in db.Centros
                          where item.UserID == USER_id
                          select item.Id).First();
                string emails = userViewModel.Email;
                string[] dominio = emails.Split('@');
                string dominiosplit = dominio[1];
                var existedominio = (from d in db.AspNetUsers where d.Email.Contains(dominiosplit) && d.Id.Equals(USER_id) select d.Id).ToList();
                if (existedominio.Count() > 0) {
                    // Add the Address Info:
                    user.Address = userViewModel.Address;
                    user.City = userViewModel.City;
                    user.State = userViewModel.State;
                    user.PostalCode = userViewModel.PostalCode;
                    user.CentroId = CE;
                    user.PasswordChange = true;
                    user.Activo = true;
                    // Then create:
                    var adminresult = await UserManager.CreateAsync(user, userViewModel.Password);

                    //Add User to the selected Roles 
                    if (adminresult.Succeeded)
                    {
                        if (selectedRoles != null)
                        {
                            var result = await UserManager.AddToRolesAsync(user.Id, selectedRoles);
                            if (!result.Succeeded)
                            {
                                resultado = result.Errors.First().ToString();
                                ModelState.AddModelError("", result.Errors.First());
                                ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
                                logsAudit.JsonResponse = resultado;
                                db.LogsAudit.Add(logsAudit);
                                db.SaveChanges();  
                                return View();
                            }
                            else {
                                resultado = "Creación de usuario consulta exitosa";
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", adminresult.Errors.First());
                        resultado =adminresult.Errors.First().ToString();
                        ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
                        logsAudit.JsonResponse = resultado;
                        db.LogsAudit.Add(logsAudit);
                        db.SaveChanges();     
                        return View();

                    }

                    logsAudit.JsonResponse = resultado;
                    db.LogsAudit.Add(logsAudit);
                    db.SaveChanges();        
                    return RedirectToAction("ListaUsers");
                }
                else
                {
                    ViewBag.mensaje = "El correo debe tener el mismo dominio del correo principal del prestador de empleo";
                    logsAudit.JsonResponse = "El correo debe tener el mismo dominio del correo principal del prestador de empleo";
                    db.LogsAudit.Add(logsAudit);
                    db.SaveChanges();     
                    return View();
                }
                
            }
            ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
            logsAudit.JsonResponse = resultado;
            db.LogsAudit.Add(logsAudit);
            db.SaveChanges();  
            return View();
        }
        //
        // GET: /Users/Editar/1
        public async Task<ActionResult> Editar(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            var userRoles = await UserManager.GetRolesAsync(user.Id);

            return View(new EditUserViewModel()
            {
                Id = user.Id,
                Email = user.Email,
                // Include the Addresss info:
                Address = user.Address,
                City = user.City,
                State = user.State,
                PostalCode = user.PostalCode,
                RolesList = RoleManager.Roles.ToList().Select(x => new SelectListItem()
                {
                    Selected = userRoles.Contains(x.Name),
                    Text = x.Name,
                    Value = x.Name
                })
            });
        }

        //
        // POST: /Users/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Editar([Bind(Include = "Email,Id,Address,City,State,PostalCode,Password,ConfirmPassword")] EditUserViewModel editUser, params string[] selectedRole)
        {
            LogsAudit logsAudit = new LogsAudit();
            logsAudit.Id = Guid.NewGuid();
            logsAudit.InstanciaServidor = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            logsAudit.UserName = User.Identity.Name;
            logsAudit.FechaHora = DateTime.Now;
            logsAudit.IpRequest = HttpContext.Request.UserHostAddress;
            logsAudit.DataRequest = HttpContext.Request.AppRelativeCurrentExecutionFilePath + " | " + HttpContext.Request.AnonymousID + " | " + HttpContext.Request.CurrentExecutionFilePathExtension + " | " + HttpContext.Request.UrlReferrer;
            logsAudit.Agent = HttpContext.Request.UserAgent;
            logsAudit.JsonRequest = JsonConvert.SerializeObject(editUser) + "||" + JsonConvert.SerializeObject(selectedRole);
            logsAudit.Operacion = "Edición de usuarios de consulta";
            string resultado = "El modelo  no es valido";
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(editUser.Id);
                if (user == null)
                {
                    return HttpNotFound();
                }

                user.UserName = editUser.Email;
                user.Email = editUser.Email;
                user.Address = editUser.Address;
                user.City = editUser.City;
                user.State = editUser.State;
                user.PostalCode = editUser.PostalCode;

                var result = UserManager.RemovePassword(user.Id);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    resultado = result.Errors.First().ToString();
                    logsAudit.JsonResponse = resultado;
                    db.LogsAudit.Add(logsAudit);
                    db.SaveChanges();  
                    return View();
                }
                result = UserManager.AddPassword(user.Id, editUser.Password);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    resultado = result.Errors.First().ToString();
                    logsAudit.JsonResponse = resultado;
                    db.LogsAudit.Add(logsAudit);
                    db.SaveChanges();  
                    return View();
                }
                logsAudit.JsonResponse = "Edición exitosa";
                db.LogsAudit.Add(logsAudit);
                db.SaveChanges();  
                return RedirectToAction("ListaUsers");
            }
            ModelState.AddModelError("", "Something failed.");
            logsAudit.JsonResponse = resultado;
            db.LogsAudit.Add(logsAudit);
            db.SaveChanges();  
            return View();
        }

        //
        // GET: /Users/Eliminar/5
        public async Task<ActionResult> Eliminar(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }
        //
        // POST: /Users/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EliminarConfirmed(string id)
        {
            LogsAudit logsAudit = new LogsAudit();
            logsAudit.Id = Guid.NewGuid();
            logsAudit.InstanciaServidor = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            logsAudit.UserName = User.Identity.Name;
            logsAudit.FechaHora = DateTime.Now;
            logsAudit.IpRequest = HttpContext.Request.UserHostAddress;
            logsAudit.DataRequest = HttpContext.Request.AppRelativeCurrentExecutionFilePath + " | " + HttpContext.Request.AnonymousID + " | " + HttpContext.Request.CurrentExecutionFilePathExtension + " | " + HttpContext.Request.UrlReferrer;
            logsAudit.Agent = HttpContext.Request.UserAgent;
            logsAudit.JsonRequest = "|User id= |" + id;
            logsAudit.Operacion = "Eliminación de usuario de consulta";
            string resultado = "El modelo no es valido";
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var user = await UserManager.FindByIdAsync(id);
                if (user == null)
                {
                    logsAudit.JsonResponse ="Usuario Null";
                    db.LogsAudit.Add(logsAudit);
                    db.SaveChanges(); 
                    return HttpNotFound();
                }
                var result = await UserManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    logsAudit.JsonResponse = result.Errors.First().ToString();
                    db.LogsAudit.Add(logsAudit);
                    db.SaveChanges(); 
                    return View();
                }
                logsAudit.JsonResponse = "El usuario a sido eliminado con exito";
                db.LogsAudit.Add(logsAudit);
                db.SaveChanges();  
                return RedirectToAction("ListaUsers");
            }
            logsAudit.JsonResponse = resultado;
            db.LogsAudit.Add(logsAudit);
            db.SaveChanges();  
            return View();
        }


        // GET: /Users/Inhabilitar/5
        public async Task<ActionResult> Inhabilitar(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        //
        // POST: /Users/Inhabilitar/5
        [HttpPost, ActionName("Inhabilitar")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> InhabilitarConfirmed(string id)
        {
            LogsAudit logsAudit = new LogsAudit();
            logsAudit.Id = Guid.NewGuid();
            logsAudit.InstanciaServidor = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            logsAudit.UserName = User.Identity.Name;
            logsAudit.FechaHora = DateTime.Now;
            logsAudit.IpRequest = HttpContext.Request.UserHostAddress;
            logsAudit.DataRequest = HttpContext.Request.AppRelativeCurrentExecutionFilePath + " | " + HttpContext.Request.AnonymousID + " | " + HttpContext.Request.CurrentExecutionFilePathExtension + " | " + HttpContext.Request.UrlReferrer;
            logsAudit.Agent = HttpContext.Request.UserAgent;
            logsAudit.JsonRequest = "Usuario Id||" + id;
            logsAudit.Operacion = "Inhabilitar usuario";
            string resultado = "El modelo  no es valido";
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    logsAudit.JsonResponse = "Usuario null";
                    db.LogsAudit.Add(logsAudit);
                    db.SaveChanges();  
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var user = await UserManager.FindByIdAsync(id);
                if (user == null)
                {
                    logsAudit.JsonResponse = "Id usuario no puede ser  null";
                    db.LogsAudit.Add(logsAudit);
                    db.SaveChanges();  
                    return HttpNotFound();
                }
                var usuario = (from us in db.AspNetUsers
                               where us.Id == user.Id
                               select us).First();
                AspNetUsers UsurioCentro = usuario;
                UsurioCentro.Activo = false;
                db.Entry(UsurioCentro).State = EntityState.Modified;
                db.SaveChanges();
                logsAudit.JsonResponse = "Usuario Inabilitado con exito";
                db.LogsAudit.Add(logsAudit);
                db.SaveChanges();  
                 if(User.IsInRole("Centros")){
                 return RedirectToAction("ListaUsers");
                 }else{
                 return RedirectToAction("Index");
                 }
                
            }
            logsAudit.JsonResponse = resultado;
            db.LogsAudit.Add(logsAudit);
            db.SaveChanges();  
            return View();
        }

        // GET: /Users/habilitar/5
        public async Task<ActionResult> habilitar(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
 
            return View(user);
        }

        //
        // POST: /Users/habilitar/5
        [HttpPost, ActionName("habilitar")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> habilitarConfirmed(string id)
        {
            LogsAudit logsAudit = new LogsAudit();
            logsAudit.Id = Guid.NewGuid();
            logsAudit.InstanciaServidor = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            logsAudit.UserName = User.Identity.Name;
            logsAudit.FechaHora = DateTime.Now;
            logsAudit.IpRequest = HttpContext.Request.UserHostAddress;
            logsAudit.DataRequest = HttpContext.Request.AppRelativeCurrentExecutionFilePath + " | " + HttpContext.Request.AnonymousID + " | " + HttpContext.Request.CurrentExecutionFilePathExtension + " | " + HttpContext.Request.UrlReferrer;
            logsAudit.Agent = HttpContext.Request.UserAgent;
            logsAudit.JsonRequest = "Usuario Id||" + id;
            logsAudit.Operacion = "Inhabilitar usuario";
            string resultado = "El modelo  no es valido";
            if (ModelState.IsValid)
            {

                if (id == null)
                {
                    logsAudit.JsonResponse = "Usuario_Id  null";
                    db.LogsAudit.Add(logsAudit);
                    db.SaveChanges();
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var user = await UserManager.FindByIdAsync(id);
                if (user == null)
                {
                    logsAudit.JsonResponse = "Usuario_Id no encontrado";
                    db.LogsAudit.Add(logsAudit);
                    db.SaveChanges();
                    return HttpNotFound();
                }
                var usuario = (from us in db.AspNetUsers
                               where us.Id == user.Id
                               select us).First();
                AspNetUsers UsurioCentro = usuario;
                UsurioCentro.Activo = true;
                db.Entry(UsurioCentro).State = EntityState.Modified;
                db.SaveChanges();
                logsAudit.JsonResponse = "Usuario habilitado con exito";
                db.LogsAudit.Add(logsAudit);
                db.SaveChanges();  
                if (User.IsInRole("Centros"))
                {
                    return RedirectToAction("ListaUsers");
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            logsAudit.JsonResponse = resultado;
            db.LogsAudit.Add(logsAudit);
            db.SaveChanges(); 
            return View();
        }
        //************************************************************************************/************************************************************************************
       [Authorize(Roles = "Admin, Unidad")]
        //
        // GET: /Users/
        public async Task<ActionResult> Index()
        {
            var usuarioPrestadores = (from r in db.AspNetRoles
                                      where r.Name == "Centros"                   
                                      select r.Id ).First();
            ViewBag.rolcentros = usuarioPrestadores.ToString();

            ViewBag.Centros = new SelectList(db.Centros, "Id", "CentroEmpleo"); 
           return View(await UserManager.Users.OrderBy(x=> x.CentroId).ToListAsync());
        }

        //
        // GET: /Users/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);

            ViewBag.RoleNames = await UserManager.GetRolesAsync(user.Id);

            return View(user);
        }

        //
        // GET: /Users/Create
        public async Task<ActionResult> Create()
        {
            //Get the list of Roles
            ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
            return View();
        }

        //
        // POST: /Users/Create
        [HttpPost]
        public async Task<ActionResult> Create(RegisterViewModel userViewModel, params string[] selectedRoles)
        {
            LogsAudit logsAudit = new LogsAudit();
            logsAudit.Id = Guid.NewGuid();
            logsAudit.InstanciaServidor = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            logsAudit.UserName = User.Identity.Name;
            logsAudit.FechaHora = DateTime.Now;
            logsAudit.IpRequest = HttpContext.Request.UserHostAddress;
            logsAudit.DataRequest = HttpContext.Request.AppRelativeCurrentExecutionFilePath + " | " + HttpContext.Request.AnonymousID + " | " + HttpContext.Request.CurrentExecutionFilePathExtension + " | " + HttpContext.Request.UrlReferrer;
            logsAudit.Agent = HttpContext.Request.UserAgent;
            logsAudit.JsonRequest = JsonConvert.SerializeObject(userViewModel) + "||" + JsonConvert.SerializeObject(selectedRoles);
            logsAudit.Operacion = "Creación de usuario centros";
            string resultado = "El modelo  no es valido";

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser 
                { 
                    UserName = userViewModel.Email, Email = 
                    userViewModel.Email, 
                    // Add the Address Info:
                    Address = userViewModel.Address,
                    City = userViewModel.City,
                    State = userViewModel.State,
                    PostalCode = userViewModel.PostalCode
                };

                // Add the Address Info:
                user.Address = userViewModel.Address;
                user.City = userViewModel.City;
                user.State = userViewModel.State;
                user.PostalCode = userViewModel.PostalCode;

                // Then create:
                var adminresult = await UserManager.CreateAsync(user, userViewModel.Password);

                //Add User to the selected Roles 
                if (adminresult.Succeeded)
                {
                    if (selectedRoles != null)
                    {
                        var result = await UserManager.AddToRolesAsync(user.Id, selectedRoles);
                        if (!result.Succeeded)
                        {
                            ModelState.AddModelError("", result.Errors.First());
                            ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
                            logsAudit.JsonResponse = result.Errors.First().ToString();
                            db.LogsAudit.Add(logsAudit);
                            db.SaveChanges();
                            return View();
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", adminresult.Errors.First());
                    ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
                    logsAudit.JsonResponse = adminresult.Errors.First().ToString();
                    db.LogsAudit.Add(logsAudit);
                    db.SaveChanges();
                    return View();

                }
                var usuario = (from us in db.AspNetUsers
                               where us.Id == user.Id
                               select us).First();
                if (usuario != null)
                {
                    Centros centro = new Centros();
                    centro.CentroEmpleo = userViewModel.CentroEmpleo;
                    centro.UserID = usuario.Id;
                    db.Centros.Add(centro);
                    db.SaveChanges();
                    AspNetUsers UsurioCentro = usuario;
                    UsurioCentro.CentroId = centro.Id;
                    UsurioCentro.PasswordChange = true;
                    UsurioCentro.Activo = true;
                    db.Entry(UsurioCentro).State = EntityState.Modified;
                    db.SaveChanges();
                }
                logsAudit.JsonResponse = "Usuario centro creado con exito";
                db.LogsAudit.Add(logsAudit);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
            userViewModel.Password = "";
            userViewModel.ConfirmPassword = "";
            logsAudit.JsonResponse = resultado;
            db.LogsAudit.Add(logsAudit);
            db.SaveChanges();
            return View(userViewModel);
        }

        //
        // GET: /Users/Edit/1
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            var userRoles = await UserManager.GetRolesAsync(user.Id);
            return View(new EditUserViewModel()
            {
                Id = user.Id,
                Email = user.Email,
                // Include the Addresss info:
                Address = user.Address,
                City = user.City,
                State = user.State,
                PostalCode = user.PostalCode,
                RolesList = RoleManager.Roles.ToList().Select(x => new SelectListItem()
                {
                    Selected = userRoles.Contains(x.Name),
                    Text = x.Name,
                    Value = x.Name
                })
            });
        }

        //
        // POST: /Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Email,Id,Address,City,State,PostalCode,Password,ConfirmPassword")] EditUserViewModel editUser, params string[] selectedRole)
        {
            var user = await UserManager.FindByIdAsync(editUser.Id);
            var userRoles = await UserManager.GetRolesAsync(user.Id);
            LogsAudit logsAudit = new LogsAudit();
            logsAudit.Id = Guid.NewGuid();
            logsAudit.InstanciaServidor = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            logsAudit.UserName = User.Identity.Name;
            logsAudit.FechaHora = DateTime.Now;
            logsAudit.IpRequest = HttpContext.Request.UserHostAddress;
            logsAudit.DataRequest = HttpContext.Request.AppRelativeCurrentExecutionFilePath + " | " + HttpContext.Request.AnonymousID + " | " + HttpContext.Request.CurrentExecutionFilePathExtension + " | " + HttpContext.Request.UrlReferrer;
            logsAudit.Agent = HttpContext.Request.UserAgent;
            logsAudit.JsonRequest = JsonConvert.SerializeObject(editUser) + "||" + JsonConvert.SerializeObject(selectedRole);
            logsAudit.Operacion = "Edición de usuarios Centros";
            string resultado = "El modelo  no es valido";
            if (ModelState.IsValid)
            {
               if (user == null)
                {
                    logsAudit.JsonResponse = "Usuario null";
                    db.LogsAudit.Add(logsAudit);
                    db.SaveChanges();
                    return HttpNotFound();
                }

                user.UserName = editUser.Email;
                user.Email = editUser.Email;
                user.Address = editUser.Address;
                user.City = editUser.City;
                user.State = editUser.State;
                user.PostalCode = editUser.PostalCode;
                selectedRole = selectedRole ?? new string[] { };

                var result = await UserManager.AddToRolesAsync(user.Id, selectedRole.Except(userRoles).ToArray<string>());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    logsAudit.JsonResponse = result.Errors.First().ToString();
                    db.LogsAudit.Add(logsAudit);
                    db.SaveChanges();
                    return View();
                }
                result = await UserManager.RemoveFromRolesAsync(user.Id, userRoles.Except(selectedRole).ToArray<string>());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    logsAudit.JsonResponse = result.Errors.First().ToString();
                    db.LogsAudit.Add(logsAudit);
                    db.SaveChanges();
                    return View();
                }
                result = UserManager.RemovePassword(user.Id);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    logsAudit.JsonResponse = result.Errors.First().ToString();
                    db.LogsAudit.Add(logsAudit);
                    db.SaveChanges();
                    return View();
                }
                result = UserManager.AddPassword(user.Id, editUser.Password);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    logsAudit.JsonResponse = result.Errors.First().ToString();
                    db.LogsAudit.Add(logsAudit);
                    db.SaveChanges();  
                    return View();
                }
                logsAudit.JsonResponse = "Edición exitosa";
                db.LogsAudit.Add(logsAudit);
                db.SaveChanges();  
                return RedirectToAction("Index");

            }
            logsAudit.JsonResponse = resultado;
            db.LogsAudit.Add(logsAudit);
            db.SaveChanges();  
            ModelState.AddModelError("", "Something failed.");
            return View(new EditUserViewModel()
            {
                Id = user.Id,
                Email = user.Email,
                // Include the Addresss info:
                Address = user.Address,
                City = user.City,
                State = user.State,
                PostalCode = user.PostalCode,
                RolesList = RoleManager.Roles.ToList().Select(x => new SelectListItem()
                {
                    Selected = userRoles.Contains(x.Name),
                    Text = x.Name,
                    Value = x.Name
                })
            });
        }

        //
        // GET: /Users/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        //
        // POST: /Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            LogsAudit logsAudit = new LogsAudit();
            logsAudit.Id = Guid.NewGuid();
            logsAudit.InstanciaServidor = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            logsAudit.UserName = User.Identity.Name;
            logsAudit.FechaHora = DateTime.Now;
            logsAudit.IpRequest = HttpContext.Request.UserHostAddress;
            logsAudit.DataRequest = HttpContext.Request.AppRelativeCurrentExecutionFilePath + " | " + HttpContext.Request.AnonymousID + " | " + HttpContext.Request.CurrentExecutionFilePathExtension + " | " + HttpContext.Request.UrlReferrer;
            logsAudit.Agent = HttpContext.Request.UserAgent;
            logsAudit.JsonRequest = "|User id= |" + id;
            logsAudit.Operacion = "Eliminación de usuario de consulta";
            string resultado = "El modelo no es valido";
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var user = await UserManager.FindByIdAsync(id);
                if (user == null)
                {
                    logsAudit.JsonResponse = "Usuario no puede ser null";
                    db.LogsAudit.Add(logsAudit);
                    db.SaveChanges();  
                    return HttpNotFound();
                }
                var userRoles = await UserManager.GetRolesAsync(id);
                string rol = userRoles.ElementAt(0);
                if (rol == "Centros")
                {
                    var CE = (from item in db.Centros
                              where item.UserID == id
                              select item.Id).First();
                    Centros centros = db.Centros.Find(CE);
                    db.Centros.Remove(centros);
                    db.SaveChanges();
                }
                var result = await UserManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    logsAudit.JsonResponse = result.Errors.First().ToString();
                    db.LogsAudit.Add(logsAudit);
                    db.SaveChanges();  
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                logsAudit.JsonResponse = "Usuario eliminado con exito";
                db.LogsAudit.Add(logsAudit);
                db.SaveChanges();  
                return RedirectToAction("Index");

            }
            logsAudit.JsonResponse = resultado;
            db.LogsAudit.Add(logsAudit);
            db.SaveChanges();  
            return View();
        }
    }
}
