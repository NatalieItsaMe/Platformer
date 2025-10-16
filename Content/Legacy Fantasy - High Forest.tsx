<?xml version="1.0" encoding="UTF-8"?>
<tileset version="1.10" tiledversion="1.11.2" name="Legacy Fantasy - High Forest" tilewidth="16" tileheight="16" spacing="3" margin="1" tilecount="625" columns="25">
 <image source="Tiles.png" trans="000000" width="474" height="474"/>
 <tile id="496">
  <properties>
   <property name="BodyType" value="Dynamic"/>
  </properties>
  <objectgroup draworder="index" id="2">
   <object id="1" x="3" y="0">
    <polygon points="0,0 0,6 -3,9 -3,13 0,16 10,16 13,13 13,9 10,6 10,0"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="498">
  <properties>
   <property name="BodyType" value="Dynamic"/>
  </properties>
  <objectgroup draworder="index" id="2">
   <object id="2" x="0" y="0" width="16" height="16">
    <ellipse/>
   </object>
  </objectgroup>
 </tile>
 <tile id="521">
  <properties>
   <property name="BodyType" value="Dynamic"/>
  </properties>
  <objectgroup draworder="index" id="2">
   <object id="1" x="6" y="0" width="4" height="8">
    <properties>
     <property name="Density" type="float" value="0.6"/>
     <property name="Friction" type="float" value="1"/>
    </properties>
   </object>
   <object id="2" x="3" y="6" width="10" height="10">
    <ellipse/>
   </object>
  </objectgroup>
 </tile>
 <wangsets>
  <wangset name="Ground" type="corner" tile="-1">
   <wangcolor name="Dirt" color="#ff0000" tile="-1" probability="1"/>
   <wangcolor name="Grass" color="#00ff00" tile="-1" probability="1"/>
   <wangtile tileid="25" wangid="0,0,0,2,0,0,0,0"/>
   <wangtile tileid="26" wangid="2,0,0,2,2,2,0,0"/>
   <wangtile tileid="27" wangid="2,0,0,2,2,2,0,0"/>
   <wangtile tileid="28" wangid="2,0,0,2,2,2,0,0"/>
   <wangtile tileid="29" wangid="0,0,0,0,0,2,0,0"/>
   <wangtile tileid="31" wangid="0,2,0,0,0,2,0,0"/>
   <wangtile tileid="32" wangid="0,0,0,2,0,0,0,2"/>
   <wangtile tileid="50" wangid="0,1,1,1,0,0,0,0"/>
   <wangtile tileid="51" wangid="1,1,1,1,1,1,1,1"/>
   <wangtile tileid="52" wangid="1,1,1,1,1,1,1,1"/>
   <wangtile tileid="53" wangid="1,1,1,1,1,1,1,1"/>
   <wangtile tileid="54" wangid="0,0,0,0,0,1,1,1"/>
   <wangtile tileid="55" wangid="2,0,0,2,0,2,0,0"/>
   <wangtile tileid="58" wangid="2,0,0,2,0,2,0,0"/>
   <wangtile tileid="75" wangid="0,1,1,1,0,0,0,0"/>
   <wangtile tileid="76" wangid="1,1,1,1,1,1,1,1"/>
   <wangtile tileid="77" wangid="1,1,1,1,1,1,1,1"/>
   <wangtile tileid="78" wangid="1,1,1,1,1,1,1,1"/>
   <wangtile tileid="79" wangid="0,0,0,0,0,1,1,1"/>
   <wangtile tileid="82" wangid="0,0,0,0,2,0,0,0"/>
   <wangtile tileid="100" wangid="0,1,0,0,0,0,0,0"/>
   <wangtile tileid="101" wangid="1,1,0,0,0,0,0,1"/>
   <wangtile tileid="102" wangid="1,1,0,0,0,0,0,1"/>
   <wangtile tileid="103" wangid="1,1,0,0,0,0,0,1"/>
   <wangtile tileid="104" wangid="0,0,0,0,0,0,0,1"/>
   <wangtile tileid="105" wangid="0,2,2,2,0,1,0,1"/>
   <wangtile tileid="106" wangid="0,1,0,1,0,2,2,2"/>
   <wangtile tileid="107" wangid="2,0,0,2,0,2,0,0"/>
   <wangtile tileid="130" wangid="0,2,0,2,0,2,0,2"/>
   <wangtile tileid="131" wangid="0,2,0,2,0,2,0,2"/>
   <wangtile tileid="132" wangid="0,2,0,2,0,2,0,2"/>
   <wangtile tileid="150" wangid="0,0,0,1,0,0,0,0"/>
   <wangtile tileid="151" wangid="0,0,0,1,0,1,0,0"/>
   <wangtile tileid="152" wangid="0,0,0,1,0,1,0,0"/>
   <wangtile tileid="153" wangid="0,0,0,1,0,1,0,0"/>
   <wangtile tileid="154" wangid="0,0,0,0,0,1,0,0"/>
   <wangtile tileid="155" wangid="0,1,0,2,0,1,0,1"/>
   <wangtile tileid="156" wangid="0,1,0,2,0,2,0,1"/>
   <wangtile tileid="157" wangid="0,1,0,1,0,2,0,1"/>
   <wangtile tileid="175" wangid="0,1,0,1,0,0,0,0"/>
   <wangtile tileid="176" wangid="0,1,0,1,0,1,0,1"/>
   <wangtile tileid="177" wangid="0,1,0,1,0,1,0,1"/>
   <wangtile tileid="178" wangid="0,1,0,1,0,1,0,1"/>
   <wangtile tileid="179" wangid="0,0,0,0,0,1,0,1"/>
   <wangtile tileid="180" wangid="0,2,0,1,0,2,0,1"/>
   <wangtile tileid="181" wangid="0,2,0,1,0,1,0,2"/>
   <wangtile tileid="182" wangid="0,1,0,2,0,1,0,2"/>
   <wangtile tileid="200" wangid="0,1,0,1,0,0,0,0"/>
   <wangtile tileid="201" wangid="0,1,0,1,0,1,0,1"/>
   <wangtile tileid="202" wangid="0,1,0,1,0,1,0,1"/>
   <wangtile tileid="203" wangid="0,1,0,1,0,1,0,1"/>
   <wangtile tileid="204" wangid="0,0,0,0,0,1,0,1"/>
   <wangtile tileid="205" wangid="0,1,0,2,0,2,0,2"/>
   <wangtile tileid="206" wangid="0,1,0,2,0,2,0,1"/>
   <wangtile tileid="207" wangid="0,2,0,2,0,2,0,1"/>
   <wangtile tileid="225" wangid="0,1,0,0,0,0,0,0"/>
   <wangtile tileid="226" wangid="0,1,0,0,0,0,0,1"/>
   <wangtile tileid="227" wangid="0,1,0,0,0,0,0,1"/>
   <wangtile tileid="228" wangid="0,1,0,0,0,0,0,1"/>
   <wangtile tileid="229" wangid="0,0,0,0,0,0,0,1"/>
   <wangtile tileid="230" wangid="0,2,0,2,0,1,0,2"/>
   <wangtile tileid="231" wangid="0,2,0,2,0,2,0,2"/>
   <wangtile tileid="232" wangid="0,2,0,1,0,2,0,2"/>
   <wangtile tileid="252" wangid="0,0,0,0,0,1,0,1"/>
   <wangtile tileid="253" wangid="0,1,0,1,0,0,0,0"/>
   <wangtile tileid="275" wangid="0,0,0,2,0,1,0,0"/>
   <wangtile tileid="276" wangid="0,0,0,1,0,2,0,0"/>
   <wangtile tileid="277" wangid="0,0,0,1,0,1,0,1"/>
   <wangtile tileid="278" wangid="0,1,0,1,0,1,0,0"/>
  </wangset>
  <wangset name="Water" type="mixed" tile="-1">
   <wangcolor name="Dirt" color="#ff0000" tile="-1" probability="1"/>
   <wangcolor name="Water" color="#0000ff" tile="-1" probability="1"/>
   <wangtile tileid="425" wangid="1,1,1,1,1,2,0,0"/>
   <wangtile tileid="426" wangid="1,1,1,1,1,1,1,1"/>
   <wangtile tileid="427" wangid="1,0,0,2,1,1,1,1"/>
   <wangtile tileid="450" wangid="1,1,2,1,1,2,2,2"/>
   <wangtile tileid="451" wangid="1,1,2,1,1,1,2,1"/>
   <wangtile tileid="452" wangid="1,2,2,2,1,1,2,1"/>
   <wangtile tileid="456" wangid="0,0,0,2,2,2,0,0"/>
   <wangtile tileid="457" wangid="0,0,0,2,2,2,0,0"/>
   <wangtile tileid="458" wangid="0,0,0,2,2,2,0,0"/>
   <wangtile tileid="459" wangid="0,0,0,2,2,2,0,0"/>
   <wangtile tileid="475" wangid="1,1,2,1,1,2,2,2"/>
   <wangtile tileid="476" wangid="1,1,2,1,1,1,2,1"/>
   <wangtile tileid="477" wangid="1,2,2,2,1,1,2,1"/>
   <wangtile tileid="481" wangid="2,2,2,2,2,2,2,2"/>
   <wangtile tileid="482" wangid="2,2,2,2,2,2,2,2"/>
   <wangtile tileid="483" wangid="2,2,2,2,2,2,2,2"/>
   <wangtile tileid="484" wangid="2,2,2,2,2,2,2,2"/>
   <wangtile tileid="500" wangid="1,1,2,1,1,2,2,2"/>
   <wangtile tileid="501" wangid="1,1,2,1,1,1,2,1"/>
   <wangtile tileid="502" wangid="1,2,2,2,1,1,2,1"/>
   <wangtile tileid="506" wangid="2,2,2,2,2,2,2,2"/>
   <wangtile tileid="507" wangid="2,2,2,2,2,2,2,2"/>
   <wangtile tileid="508" wangid="2,2,2,2,2,2,2,2"/>
   <wangtile tileid="509" wangid="2,2,2,2,2,2,2,2"/>
   <wangtile tileid="525" wangid="0,0,0,1,0,0,0,0"/>
   <wangtile tileid="526" wangid="0,0,0,1,1,1,0,0"/>
   <wangtile tileid="527" wangid="0,0,0,1,1,1,0,0"/>
   <wangtile tileid="528" wangid="0,0,0,1,1,1,0,0"/>
   <wangtile tileid="529" wangid="0,0,0,0,0,1,0,0"/>
   <wangtile tileid="535" wangid="0,0,0,1,0,0,0,0"/>
   <wangtile tileid="536" wangid="0,0,0,1,1,1,0,0"/>
   <wangtile tileid="537" wangid="0,0,0,1,1,1,0,0"/>
   <wangtile tileid="538" wangid="0,0,0,1,1,1,0,0"/>
   <wangtile tileid="539" wangid="0,0,0,0,0,1,0,0"/>
   <wangtile tileid="550" wangid="0,1,1,1,1,0,0,0"/>
   <wangtile tileid="551" wangid="1,1,1,1,1,1,1,1"/>
   <wangtile tileid="552" wangid="1,1,1,1,1,1,1,1"/>
   <wangtile tileid="553" wangid="1,1,1,1,1,1,1,1"/>
   <wangtile tileid="554" wangid="0,0,0,0,1,1,1,1"/>
   <wangtile tileid="560" wangid="0,1,1,1,1,0,0,0"/>
   <wangtile tileid="561" wangid="1,1,1,1,1,1,1,1"/>
   <wangtile tileid="562" wangid="1,1,1,1,1,1,1,1"/>
   <wangtile tileid="563" wangid="1,1,1,1,1,1,1,1"/>
   <wangtile tileid="564" wangid="0,0,0,0,1,1,1,1"/>
  </wangset>
  <wangset name="Leaves" type="corner" tile="-1">
   <wangcolor name="Leaves" color="#00aa00" tile="-1" probability="1"/>
   <wangtile tileid="300" wangid="0,1,0,0,0,1,0,1"/>
   <wangtile tileid="301" wangid="0,1,0,1,0,0,0,1"/>
   <wangtile tileid="302" wangid="0,1,0,1,0,1,0,1"/>
   <wangtile tileid="303" wangid="0,1,0,1,0,1,0,1"/>
   <wangtile tileid="325" wangid="0,0,0,1,0,1,0,1"/>
   <wangtile tileid="326" wangid="0,1,0,1,0,1,0,0"/>
   <wangtile tileid="327" wangid="0,1,0,1,0,1,0,1"/>
   <wangtile tileid="328" wangid="0,1,0,1,0,1,0,1"/>
   <wangtile tileid="350" wangid="0,0,0,1,0,0,0,0"/>
   <wangtile tileid="351" wangid="0,0,0,1,0,1,0,0"/>
   <wangtile tileid="352" wangid="0,0,0,0,0,1,0,0"/>
   <wangtile tileid="375" wangid="0,1,0,1,0,0,0,0"/>
   <wangtile tileid="376" wangid="0,1,0,1,0,1,0,1"/>
   <wangtile tileid="377" wangid="0,0,0,0,0,1,0,1"/>
   <wangtile tileid="400" wangid="0,1,0,0,0,0,0,0"/>
   <wangtile tileid="401" wangid="0,1,0,0,0,0,0,1"/>
   <wangtile tileid="402" wangid="0,0,0,0,0,0,0,1"/>
  </wangset>
 </wangsets>
</tileset>
