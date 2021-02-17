<?xml version="1.0"?> 
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    version="1.0">
	<xsl:output method="html" indent="yes" version="4.0"/>
	<xsl:template match="/">

		<html>
			<body style="font-family:Arial;font-size:12pt;background-color:#EEEEEE">
			 <h2>Bankiniai pavedimai</h2>
				<h4>Organizacija: <xsl:value-of select="Mokėjimai/Organizacija"/></h4>
				<h4>Data: <xsl:value-of select="Mokėjimai/Data"/></h4>
				<table border="1">
				<tr bgcolor="#9acd32">
				<th style="text-align:left">Iš sąskaitos</th>
				<th style="text-align:left">Kam</th>
				<th style="text-align:left">Gavėjo bankas, sąskaita</th>
				<th style="text-align:left">Suma</th>
				</tr>
				<xsl:for-each select="Mokėjimai/Pavedimas">
					
					<tr>
						<td><xsl:value-of select="Sąskaita"/></td>
						<td><xsl:value-of select="Gavėjas/Pavadinimas"/></td>
						<td><xsl:value-of select="Gavėjas/Bankas"/>,
							<xsl:value-of select="Gavėjas/Sąskaita"/></td>
						<td><xsl:value-of select="Suma"/><xsl:text> </xsl:text>
							<xsl:value-of select="Suma/@valiuta"/></td>
					</tr>
				</xsl:for-each>
				</table>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>
