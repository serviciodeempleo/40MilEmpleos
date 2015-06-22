<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
  <xsl:output method="html" indent="yes"/>
  
  <!-- bootstrap3.form.element.input -->
  <xsl:template name="bootstrap3.form.element.input">
    <xsl:param name="field_label"/>
    <xsl:param name="label_class"/>
    <xsl:param name="field_name"/>
    <xsl:param name="field_value"/>
    <xsl:param name="field_placeholder"/>
    <xsl:param name="field_class"/>
    <xsl:param name="field_maxlength"/>
    <xsl:param name="field_type"/>
    <xsl:param name="field_disabled"/>
    <xsl:param name="field_readonly"/>
    <xsl:param name="field_required"/>
    <xsl:param name="input-group-addon"/>

    <xsl:param name="data-date-format"/>
    <xsl:param name="data-link-format"/>

    <!-- label ? -->
    <xsl:if test="string($field_label) != ''">
      <label for="{$field_name}">
        <!-- label -> class -->
        <xsl:if test="string($label_class) != ''">
          <xsl:attribute name="class">
            <xsl:value-of select="$label_class"/>
          </xsl:attribute>
        </xsl:if>
        <!-- if date -->
        <xsl:if test="string($field_type) = 'date'">
          <xsl:attribute name="for">
            <xsl:value-of select="$field_name"/>
            <xsl:text>_helper</xsl:text>
          </xsl:attribute>
        </xsl:if>
        <xsl:value-of select="$field_label"/>
      </label>
    </xsl:if>
    <!-- input container -->
    <div>

      <!-- if date -->
      <xsl:if test="string($field_type) = 'date'">
        <xsl:attribute name="data-link-field">
          <xsl:value-of select="string($field_name)"/>
        </xsl:attribute>
        <xsl:attribute name="data-date-format">
          <xsl:value-of select="string($data-date-format)"/>
        </xsl:attribute>
        <xsl:attribute name="data-link-format">
          <xsl:value-of select="string($data-link-format)"/>
        </xsl:attribute>
      </xsl:if>

      <!-- input container -> class -->
      <xsl:attribute name="class">
        <xsl:if test="string($field_class) != ''">
          <xsl:text> </xsl:text>
          <xsl:value-of select="$field_class"/>
        </xsl:if>
        <xsl:if test="string($input-group-addon) != '' or string($field_type) = 'date'">
          <xsl:text> input-group date</xsl:text>
        </xsl:if>
      </xsl:attribute>

      <!-- input field -->
      <input name="{$field_name}" id="{$field_name}" class="form-control">


        <xsl:choose>
          <!-- if date > change id & change name (to helper field) -->
          <xsl:when test="string($field_type) = 'date'">
            <xsl:attribute name="id">
              <xsl:value-of select="string($field_name)"/>
              <xsl:text>_helper</xsl:text>
            </xsl:attribute>
            <xsl:attribute name="name">
              <xsl:value-of select="string($field_name)"/>
              <xsl:text>_helper</xsl:text>
            </xsl:attribute>
          </xsl:when>
          <!-- else if requred > add required attribute -->
          <xsl:when test="string($field_required) = 'true'">
            <xsl:attribute name="data-rule-required">true</xsl:attribute>
          </xsl:when>
        </xsl:choose>


        <!-- input field > type -->
        <xsl:attribute name="type">
          <xsl:choose>
            <!-- type -> $field_type -->
            <xsl:when test="string($field_type) != '' and string($field_type) != 'date'">
              <xsl:value-of select="$field_type"/>
            </xsl:when>
            <xsl:otherwise>
              <!-- type -> default -->
              <xsl:text>text</xsl:text>
            </xsl:otherwise>
          </xsl:choose>
        </xsl:attribute>
        <!-- input field > placeholder -->
        <xsl:if test="string($field_placeholder) != ''">
          <xsl:attribute name="placeholder">
            <xsl:value-of select="$field_placeholder"/>
          </xsl:attribute>
        </xsl:if>
        <!-- input field > value -->
        <xsl:attribute name="value">
          <xsl:value-of select="$field_value"/>
        </xsl:attribute>
        <!-- input field > maxlength -->
        <xsl:if test="string($field_maxlength) != ''">
          <xsl:attribute name="maxlength">
            <xsl:value-of select="$field_maxlength"/>
          </xsl:attribute>
        </xsl:if>
        <!-- input field > disabled ?-->
        <xsl:if test="string($field_disabled) = 'true'">
          <xsl:attribute name="disabled">
            <xsl:value-of select="''"/>
          </xsl:attribute>
        </xsl:if>
        <!-- input field > readonly -->
        <xsl:if test="string($field_readonly) = 'true'">
          <xsl:attribute name="readonly">
            <xsl:value-of select="''"/>
          </xsl:attribute>
        </xsl:if>
      </input>

      <!-- input-group-addon -->
      <xsl:choose>
        <!-- if group-addon provided -->
        <xsl:when test="string($input-group-addon) != ''">
          <span class="input-group-addon">
            <xsl:value-of disable-output-escaping="yes" select="$input-group-addon"/>
          </span>
        </xsl:when>
        <!-- else if date-->
        <xsl:when test="string($field_type) = 'date'">
          <!-- add calendar-->
          <span class="input-group-addon">
            <span class="glyphicon glyphicon-calendar"></span>
          </span>
        </xsl:when>
      </xsl:choose>
      <!-- type date -->
      <xsl:if test="string($field_type) = 'date'">
        <input type="hidden" data-rule-date="true" id="{$field_name}" name="{$field_name}" value="">
          <xsl:if test="string($field_required) = 'true'">
            <xsl:attribute name="data-rule-required">true</xsl:attribute>
          </xsl:if>
        </input>
      </xsl:if>
    </div>


  </xsl:template>

  <!-- bootstrap3.form.element.select -->
  <xsl:template name="bootstrap3.form.element.select">
    <xsl:param name="field_label"/>
    <xsl:param name="field_name"/>
    <xsl:param name="field_value"/>
    <xsl:param name="field_multiple"/>
    <xsl:param name="label_class"/>
    <xsl:param name="field_class"/>
    <xsl:param name="data_node"/>
    <xsl:param name="field_type"/>

    <label for="{$field_name}">
      <xsl:if test="string($label_class) != ''">
        <xsl:attribute name="class">
          <xsl:value-of select="$label_class"/>
        </xsl:attribute>
      </xsl:if>
      <xsl:value-of select="$field_label"/>
    </label>
    <div>
      <xsl:if test="string($field_class) != ''">
        <xsl:attribute name="class">
          <xsl:text> </xsl:text>
          <xsl:value-of select="$field_class"/>
        </xsl:attribute>
      </xsl:if>
      <select name="{$field_name}" id="{$field_name}" style="width:100%;">
        <xsl:attribute name="class">
          <xsl:choose>
            <xsl:when test="string($field_type) = 'SELECT2'">
              <!-- add select2 in js file -->
              <xsl:text>form-control add_select2</xsl:text>
            </xsl:when>
            <xsl:otherwise>
              <!-- standard bootstrap dropdown -->
              <xsl:text>form-control</xsl:text>
            </xsl:otherwise>
          </xsl:choose>
        </xsl:attribute>
        <xsl:if test="string($field_multiple) = 'true' or string($field_multiple) = 'True'">
          <xsl:attribute name="multiple">
            <xsl:value-of select="''"/>
          </xsl:attribute>
        </xsl:if>
        <xsl:choose>
          <xsl:when test="string($field_name) = 'MENU_ID'">
            <option></option>
            <xsl:apply-templates select="/ROOT/MENUS/ARRAY[string(PARENT_ID) = '']">
              <xsl:with-param name="space" select="''"/>
            </xsl:apply-templates>
          </xsl:when>
          <xsl:when test="string($field_name) = 'CUSTOM_XML_KEYWORD'">
            <xsl:apply-templates select="/ROOT/KEYWORD/ARRAY[string(PARENT_ID) = '']">
              <xsl:with-param name="space" select="''"/>
            </xsl:apply-templates>
          </xsl:when>
          <xsl:otherwise>
            <xsl:if test="string($data_node) != ''">
              <xsl:for-each select="$data_node">
                <option value="{data_key}">
                  <!-- selected on multiple select not implemented -->
                  <xsl:if test="string($field_value) = string(data_key)">
                    <xsl:attribute name="selected">selected</xsl:attribute>
                  </xsl:if>
                  <xsl:value-of select="data_value"/>
                </option>
              </xsl:for-each>
            </xsl:if>
          </xsl:otherwise>
        </xsl:choose>
      </select>
    </div>


  </xsl:template>

  <!--<xsl:template match="@* | node()">
        <xsl:copy>
            <xsl:apply-templates select="@* | node()"/>
        </xsl:copy>
    </xsl:template>-->
  
</xsl:stylesheet>
