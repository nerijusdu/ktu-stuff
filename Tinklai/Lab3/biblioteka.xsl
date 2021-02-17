<?xml version="1.0"?> 
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    version="1.0">
	<xsl:output method="html" indent="yes" version="4.0"/>
	<xsl:template match="/">
		<html>
			<body style="font-family:Arial;font-size:12pt;background-color:#EEEEEE">
			    <h2>Biblioteka</h2>
				<table border="1">
                    <tr bgcolor="#9acd32">
                    <th style="text-align:left">Pavadinimas</th>
                    <th style="text-align:left">Autorius</th>
                    <th style="text-align:left">Išleidimo metai</th>
                    <th style="text-align:left">Egzempliorių skaičius</th>
                    </tr>
                    <xsl:for-each select="Biblioteka/Knyga">
                        <tr>
                            <td><xsl:value-of select="Pavadinimas"/></td>
                            <td><xsl:value-of select="Autorius"/></td>
                            <td><xsl:value-of select="Metai"/></td>
                            <td><xsl:value-of select="Skaičius"/></td>
                        </tr>
                    </xsl:for-each>
				</table>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>