﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Moq;
using NeuToDo.Services;
using NeuToDo.Utils;
using NUnit.Framework;

namespace NeuToDo.UnitTest.Services
{
    public class NeuSyllabusGetterTest
    {
        [Test]
        public void JsParseTest()
        {
            var responseBody =
                "\r\n<form id=\"exportTableForm\" name=\"exportTableForm\" action=\"/eams/courseTableForStd!courseTable.action\" method=\"post\" target=\"\" >\r\n\r\n \r\n<script language=\"JavaScript\" type=\"text/JavaScript\" src=\"/eams/static/scripts/course/TaskActivity.js?v3.21\"></script>\r\n<script language=\"JavaScript\">\r\n  function fillTable(table,weeks,units,tableIndex){\r\n  \r\n       for(var i=0;i<weeks;i++){\r\n        for(var j=0;j<units-1;j++){\r\n            var index =units*i+j;\r\n            var preTd=jQuery(\"#TD\"+index+\"_\"+tableIndex);\r\n            var nextTd=jQuery(\"#TD\"+(index+1)+\"_\"+tableIndex);\r\n            while(table.marshalContents[index]!=null&&table.marshalContents[index+1]!=null&&table.marshalContents[index]==table.marshalContents[index+1]){\r\n                nextTd.remove();\r\n                var spanNumber = 1;\r\n                if(preTd.prop(\"rowSpan\")) spanNumber = new Number(preTd.prop(\"rowSpan\"))\r\n                spanNumber++;\r\n                preTd.prop(\"rowSpan\",spanNumber);\r\n                j++;\r\n                if(j>=units-1){\r\n                    break;\r\n                }\r\n                index=index+1;\r\n                nextTd=jQuery(\"#TD\"+(index+1)+\"_\"+tableIndex);\r\n            }\r\n        }\r\n      }\r\n      \r\n        for(var k = 0; k < table.unitCounts; k++){\r\n      var td=document.getElementById(\"TD\" + k + \"_\" + tableIndex);\r\n      if(td != null && table.marshalContents[k] != null) { \r\n        td.innerHTML = table.marshalContents[k];\r\n        td.style.backgroundColor=\"#94aef3\";\r\n        td.className = \"infoTitle\";\r\n        \r\n        // 查找冲突 table.activities是什么，可以查看TaskActivity.js和courseTableContent_script.ftl\r\n        var activitiesInCell = table.activities[k];\r\n        if(detectCollisionInCell(activitiesInCell)) {\r\n          td.style.backgroundColor=\"red\";\r\n        }\r\n        td.className=\"infoTitle\";\r\n        td.title=table.marshalContents[k].replace(/<br>/g,\";\");\r\n      }\r\n    }\r\n    }\r\nfunction detectCollisionInCell(activitiesInCell) {\r\n  if(activitiesInCell.length <=1)\r\n    return false;\r\n    var isAllBigCourse = true;\r\n   for (var i = 0; i < activitiesInCell.length; i++) {\r\n    if (activitiesInCell[i].remark !== '大课排课') {\r\n        isAllBigCourse = false;\r\n      }\r\n   }\r\n   if (isAllBigCourse) {\r\n    return false;\r\n   }\r\n   \r\n   \r\n  //单元格的课程集合[courseId(seqNo)->true]\r\n  var cellCourseIds=new Object();\r\n  var mergedValidWeeks = activitiesInCell[0].vaildWeeks.split(\"\");\r\n  //登记第一个课程\r\n  cellCourseIds[ activitiesInCell[0].courseName]=true;\r\n  for(var z = 1; z < activitiesInCell.length; z++) {    \r\n    var detectCollision = false;\r\n    var tValidWeeks = activitiesInCell[z].vaildWeeks.split(\"\");\r\n    if(mergedValidWeeks.length != tValidWeeks.length) {\r\n      alert('mergedValidWeeks.length != tValidWeeks.length');\r\n      return;\r\n    }\r\n    for(var x = 0; x < mergedValidWeeks.length; x++) {  //53代表53周\r\n      var m = new Number(mergedValidWeeks[x]);\r\n      var t = new Number(tValidWeeks[x]);\r\n      if(m + t <= 1) {\r\n        mergedValidWeeks[x] = m + t;\r\n      }\r\n      else {\r\n        //如果已经是登记过的，则不算冲突\r\n          if(!cellCourseIds[activitiesInCell[z].courseName]){\r\n            return true;  //发现冲突\r\n          }\r\n        }\r\n      }\r\n      //登记该课程\r\n      cellCourseIds[activitiesInCell[z].courseName]=true;\r\n  }\r\n  return false;\r\n}\r\n</script>\r\n<div id=\"toolbar12042826911\"></div>\r\n<script type=\"text/javascript\">\r\n  bar = bg.ui.toolbar(\"toolbar12042826911\",'');\r\n  \t\t\tbar.addItem(\"打印\",'bg.Go(\"/eams/courseTableForStd!courseTable.action?setting.kind=std&ids=14625\", \"_blank\")');\r\n\t\t\tbar.addItem(\"导出\",'bg.Go(\"/eams/courseTableForStd!exportStdCourseGrade.action?setting.kind=std&ids=14625\", \"_blank\")');\r\n\r\n\r\n  bar.addHr();\r\n</script>\r\n<div id = \"ExportA\" width=\"100%\" align=\"center\" cellpadding=\"0\" cellspacing=\"0\">\r\n      <pre>课表格式说明：教师姓名 课程名称(序号) (第n周-第m周,教室)</pre>\r\n    <table width=\"100%\" id=\"manualArrangeCourseTable\" align=\"center\" class=\"gridtable\"  style=\"text-align:center\" border=\"1\">\r\n      <thead>\r\n      <tr>\r\n          <th style=\"background-color:#DEEDF7;\" height=\"10px\" width=\"80px\">节次/周次</td>\r\n          <th style=\"background-color:#DEEDF7;\">\r\n              <font size=\"2px\">星期日</font>\r\n        </th>\r\n          <th style=\"background-color:#DEEDF7;\">\r\n              <font size=\"2px\">星期一</font>\r\n        </th>\r\n          <th style=\"background-color:#DEEDF7;\">\r\n              <font size=\"2px\">星期二</font>\r\n        </th>\r\n          <th style=\"background-color:#DEEDF7;\">\r\n              <font size=\"2px\">星期三</font>\r\n        </th>\r\n          <th style=\"background-color:#DEEDF7;\">\r\n              <font size=\"2px\">星期四</font>\r\n        </th>\r\n          <th style=\"background-color:#DEEDF7;\">\r\n              <font size=\"2px\">星期五</font>\r\n        </th>\r\n          <th style=\"background-color:#DEEDF7;\">\r\n              <font size=\"2px\">星期六</font>\r\n        </th>\r\n      </tr>\r\n      </thead>\r\n      <tr>\r\n          <td style=\"background-color:#EEFF00\">\r\n            <font size=\"2px\"> 第一节</font>\r\n        </td>\r\n          <td id=\"TD72_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD0_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD12_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD24_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD36_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD48_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD60_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n      </tr>\r\n      <tr>\r\n          <td style=\"background-color:#EEFF00\">\r\n            <font size=\"2px\"> 第二节</font>\r\n        </td>\r\n          <td id=\"TD73_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD1_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD13_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD25_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD37_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD49_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD61_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n      </tr>\r\n      <tr>\r\n          <td style=\"background-color:#EEFF00\">\r\n            <font size=\"2px\"> 第三节</font>\r\n        </td>\r\n          <td id=\"TD74_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD2_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD14_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD26_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD38_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD50_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD62_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n      </tr>\r\n      <tr>\r\n          <td style=\"background-color:#EEFF00\">\r\n            <font size=\"2px\"> 第四节</font>\r\n        </td>\r\n          <td id=\"TD75_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD3_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD15_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD27_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD39_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD51_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD63_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n      </tr>\r\n      <tr>\r\n          <td style=\"background-color:#33BB00\">\r\n            <font size=\"2px\"> 第五节</font>\r\n        </td>\r\n          <td id=\"TD76_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD4_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD16_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD28_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD40_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD52_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD64_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n      </tr>\r\n      <tr>\r\n          <td style=\"background-color:#33BB00\">\r\n            <font size=\"2px\"> 第六节</font>\r\n        </td>\r\n          <td id=\"TD77_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD5_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD17_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD29_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD41_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD53_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD65_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n      </tr>\r\n      <tr>\r\n          <td style=\"background-color:#33BB00\">\r\n            <font size=\"2px\"> 第七节</font>\r\n        </td>\r\n          <td id=\"TD78_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD6_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD18_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD30_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD42_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD54_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD66_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n      </tr>\r\n      <tr>\r\n          <td style=\"background-color:#33BB00\">\r\n            <font size=\"2px\"> 第八节</font>\r\n        </td>\r\n          <td id=\"TD79_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD7_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD19_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD31_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD43_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD55_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD67_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n      </tr>\r\n      <tr>\r\n          <td style=\"background-color:pink\">\r\n            <font size=\"2px\"> 第九节</font>\r\n        </td>\r\n          <td id=\"TD80_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD8_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD20_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD32_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD44_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD56_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD68_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n      </tr>\r\n      <tr>\r\n          <td style=\"background-color:pink\">\r\n            <font size=\"2px\"> 第十节</font>\r\n        </td>\r\n          <td id=\"TD81_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD9_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD21_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD33_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD45_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD57_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD69_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n      </tr>\r\n      <tr>\r\n          <td style=\"background-color:pink\">\r\n            <font size=\"2px\"> 第十一节</font>\r\n        </td>\r\n          <td id=\"TD82_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD10_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD22_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD34_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD46_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD58_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD70_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n      </tr>\r\n      <tr>\r\n          <td style=\"background-color:pink\">\r\n            <font size=\"2px\"> 第十二节</font>\r\n        </td>\r\n          <td id=\"TD83_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD11_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD23_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD35_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD47_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD59_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n          <td id=\"TD71_0\"  style=\"backGround-Color:#ffffff;font-size:12px\"></td>\r\n      </tr>\r\n    </table> \r\n<script language=\"JavaScript\">\r\n\t// function CourseTable in TaskActivity.js\r\n\tvar language = \"zh\";\r\n\tvar table0 = new CourseTable(2020,84);\r\n\tvar unitCount = 12;\r\n\tvar index=0;\r\n\tvar activity=null;\r\n\t\tvar teachers = [{id:9638,name:\"赵建喆\",lab:false}];\r\n\t\tvar actTeachers = [{id:9638,name:\"赵建喆\",lab:false}];\r\n\t\tvar assistant = _.filter(actTeachers, function(actTeacher) {\r\n\t\t\treturn (_.where(teachers, {id:actTeacher.id,name:actTeacher.name,lab:actTeacher.lab}).length == 0) && (actTeacher.lab == true);\r\n\t\t});\r\n\t\tvar assistantName = \"\";\r\n\t\tif (assistant.length > 0) {\r\n\t\t\tassistantName = assistant[0].name;\r\n\t\t\tactTeachers = _.reject(actTeachers, function(actTeacher) {\r\n\t\t\t\treturn _.where(assistant, {id:actTeacher.id}).length > 0;\r\n\t\t\t});\r\n\t\t}\r\n\t\tvar actTeacherId = [];\r\n\t\tvar actTeacherName = [];\r\n\t\tfor (var i = 0; i < actTeachers.length; i++) {\r\n\t\t\tactTeacherId.push(actTeachers[i].id);\r\n\t\t\tactTeacherName.push(actTeachers[i].name);\r\n\t\t}\r\n\t\t\tactivity = new TaskActivity(actTeacherId.join(','),actTeacherName.join(','),\"87929(A016810)\",\"人机交互的软件工程方法(A016810-线上非实时授课)\",\"453\",\"信息A101(浑南校区)\",\"01111111100000000000000000000000000000000000000000000\",null,null,assistantName,\"\",\"\");\r\n\t\t\tindex =2*unitCount+0;\r\n\t\t\ttable0.activities[index][table0.activities[index].length]=activity;\r\n\t\t\tindex =2*unitCount+1;\r\n\t\t\ttable0.activities[index][table0.activities[index].length]=activity;\r\n\t\tvar teachers = [{id:9638,name:\"赵建喆\",lab:false}];\r\n\t\tvar actTeachers = [{id:9638,name:\"赵建喆\",lab:false}];\r\n\t\tvar assistant = _.filter(actTeachers, function(actTeacher) {\r\n\t\t\treturn (_.where(teachers, {id:actTeacher.id,name:actTeacher.name,lab:actTeacher.lab}).length == 0) && (actTeacher.lab == true);\r\n\t\t});\r\n\t\tvar assistantName = \"\";\r\n\t\tif (assistant.length > 0) {\r\n\t\t\tassistantName = assistant[0].name;\r\n\t\t\tactTeachers = _.reject(actTeachers, function(actTeacher) {\r\n\t\t\t\treturn _.where(assistant, {id:actTeacher.id}).length > 0;\r\n\t\t\t});\r\n\t\t}\r\n\t\tvar actTeacherId = [];\r\n\t\tvar actTeacherName = [];\r\n\t\tfor (var i = 0; i < actTeachers.length; i++) {\r\n\t\t\tactTeacherId.push(actTeachers[i].id);\r\n\t\t\tactTeacherName.push(actTeachers[i].name);\r\n\t\t}\r\n\t\t\tactivity = new TaskActivity(actTeacherId.join(','),actTeacherName.join(','),\"87929(A016810)\",\"人机交互的软件工程方法(A016810-线上非实时授课)\",\"453\",\"信息A101(浑南校区)\",\"01111111100000000000000000000000000000000000000000000\",null,null,assistantName,\"\",\"\");\r\n\t\t\tindex =0*unitCount+0;\r\n\t\t\ttable0.activities[index][table0.activities[index].length]=activity;\r\n\t\t\tindex =0*unitCount+1;\r\n\t\t\ttable0.activities[index][table0.activities[index].length]=activity;\r\n\t\tvar teachers = [{id:10555,name:\"费园园\",lab:false}];\r\n\t\tvar actTeachers = [{id:10555,name:\"费园园\",lab:false}];\r\n\t\tvar assistant = _.filter(actTeachers, function(actTeacher) {\r\n\t\t\treturn (_.where(teachers, {id:actTeacher.id,name:actTeacher.name,lab:actTeacher.lab}).length == 0) && (actTeacher.lab == true);\r\n\t\t});\r\n\t\tvar assistantName = \"\";\r\n\t\tif (assistant.length > 0) {\r\n\t\t\tassistantName = assistant[0].name;\r\n\t\t\tactTeachers = _.reject(actTeachers, function(actTeacher) {\r\n\t\t\t\treturn _.where(assistant, {id:actTeacher.id}).length > 0;\r\n\t\t\t});\r\n\t\t}\r\n\t\tvar actTeacherId = [];\r\n\t\tvar actTeacherName = [];\r\n\t\tfor (var i = 0; i < actTeachers.length; i++) {\r\n\t\t\tactTeacherId.push(actTeachers[i].id);\r\n\t\t\tactTeacherName.push(actTeachers[i].name);\r\n\t\t}\r\n\t\t\tactivity = new TaskActivity(actTeacherId.join(','),actTeacherName.join(','),\"87999(A016518)\",\"服务工程方法论(A016518-线上实时、非实时结合授课)\",\"453\",\"信息A101(浑南校区)\",\"01110001100000000000000000000000000000000000000000000\",null,null,assistantName,\"\",\"\");\r\n\t\t\tindex =5*unitCount+4;\r\n\t\t\ttable0.activities[index][table0.activities[index].length]=activity;\r\n\t\t\tindex =5*unitCount+5;\r\n\t\t\ttable0.activities[index][table0.activities[index].length]=activity;\r\n\t\t\tindex =5*unitCount+6;\r\n\t\t\ttable0.activities[index][table0.activities[index].length]=activity;\r\n\t\t\tindex =5*unitCount+7;\r\n\t\t\ttable0.activities[index][table0.activities[index].length]=activity;\r\n\t\tvar teachers = [{id:10555,name:\"费园园\",lab:false}];\r\n\t\tvar actTeachers = [{id:10555,name:\"费园园\",lab:false}];\r\n\t\tvar assistant = _.filter(actTeachers, function(actTeacher) {\r\n\t\t\treturn (_.where(teachers, {id:actTeacher.id,name:actTeacher.name,lab:actTeacher.lab}).length == 0) && (actTeacher.lab == true);\r\n\t\t});\r\n\t\tvar assistantName = \"\";\r\n\t\tif (assistant.length > 0) {\r\n\t\t\tassistantName = assistant[0].name;\r\n\t\t\tactTeachers = _.reject(actTeachers, function(actTeacher) {\r\n\t\t\t\treturn _.where(assistant, {id:actTeacher.id}).length > 0;\r\n\t\t\t});\r\n\t\t}\r\n\t\tvar actTeacherId = [];\r\n\t\tvar actTeacherName = [];\r\n\t\tfor (var i = 0; i < actTeachers.length; i++) {\r\n\t\t\tactTeacherId.push(actTeachers[i].id);\r\n\t\t\tactTeacherName.push(actTeachers[i].name);\r\n\t\t}\r\n\t\t\tactivity = new TaskActivity(actTeacherId.join(','),actTeacherName.join(','),\"87999(A016518)\",\"服务工程方法论(A016518-线上实时、非实时结合授课)\",\"453\",\"信息A101(浑南校区)\",\"00111000000000000000000000000000000000000000000000000\",null,null,assistantName,\"\",\"\");\r\n\t\t\tindex =6*unitCount+4;\r\n\t\t\ttable0.activities[index][table0.activities[index].length]=activity;\r\n\t\t\tindex =6*unitCount+5;\r\n\t\t\ttable0.activities[index][table0.activities[index].length]=activity;\r\n\t\t\tindex =6*unitCount+6;\r\n\t\t\ttable0.activities[index][table0.activities[index].length]=activity;\r\n\t\t\tindex =6*unitCount+7;\r\n\t\t\ttable0.activities[index][table0.activities[index].length]=activity;\r\n\t\tvar teachers = [{id:7193,name:\"李正鸿\",lab:false},{id:6881,name:\"陈红兵\",lab:false}];\r\n\t\tvar actTeachers = [{id:7193,name:\"李正鸿\",lab:false},{id:6881,name:\"陈红兵\",lab:false}];\r\n\t\tvar assistant = _.filter(actTeachers, function(actTeacher) {\r\n\t\t\treturn (_.where(teachers, {id:actTeacher.id,name:actTeacher.name,lab:actTeacher.lab}).length == 0) && (actTeacher.lab == true);\r\n\t\t});\r\n\t\tvar assistantName = \"\";\r\n\t\tif (assistant.length > 0) {\r\n\t\t\tassistantName = assistant[0].name;\r\n\t\t\tactTeachers = _.reject(actTeachers, function(actTeacher) {\r\n\t\t\t\treturn _.where(assistant, {id:actTeacher.id}).length > 0;\r\n\t\t\t});\r\n\t\t}\r\n\t\tvar actTeacherId = [];\r\n\t\tvar actTeacherName = [];\r\n\t\tfor (var i = 0; i < actTeachers.length; i++) {\r\n\t\t\tactTeacherId.push(actTeachers[i].id);\r\n\t\t\tactTeacherName.push(actTeachers[i].name);\r\n\t\t}\r\n\t\t\tactivity = new TaskActivity(actTeacherId.join(','),actTeacherName.join(','),\"95342(A019378)\",\"全球视野下的人类文明与科技发展(A019378-线上实时授课)\",\"434\",\"生命B401(浑南校区)\",\"01111000000000000000000000000000000000000000000000000\",null,null,assistantName,\"\",\"\");\r\n\t\t\tindex =2*unitCount+8;\r\n\t\t\ttable0.activities[index][table0.activities[index].length]=activity;\r\n\t\t\tindex =2*unitCount+9;\r\n\t\t\ttable0.activities[index][table0.activities[index].length]=activity;\r\n\t\t\tindex =2*unitCount+10;\r\n\t\t\ttable0.activities[index][table0.activities[index].length]=activity;\r\n\t\t\tindex =2*unitCount+11;\r\n\t\t\ttable0.activities[index][table0.activities[index].length]=activity;\r\n\t\tvar teachers = [{id:10545,name:\"凌棕\",lab:false}];\r\n\t\tvar actTeachers = [{id:10545,name:\"凌棕\",lab:false}];\r\n\t\tvar assistant = _.filter(actTeachers, function(actTeacher) {\r\n\t\t\treturn (_.where(teachers, {id:actTeacher.id,name:actTeacher.name,lab:actTeacher.lab}).length == 0) && (actTeacher.lab == true);\r\n\t\t});\r\n\t\tvar assistantName = \"\";\r\n\t\tif (assistant.length > 0) {\r\n\t\t\tassistantName = assistant[0].name;\r\n\t\t\tactTeachers = _.reject(actTeachers, function(actTeacher) {\r\n\t\t\t\treturn _.where(assistant, {id:actTeacher.id}).length > 0;\r\n\t\t\t});\r\n\t\t}\r\n\t\tvar actTeacherId = [];\r\n\t\tvar actTeacherName = [];\r\n\t\tfor (var i = 0; i < actTeachers.length; i++) {\r\n\t\t\tactTeacherId.push(actTeachers[i].id);\r\n\t\t\tactTeacherName.push(actTeachers[i].name);\r\n\t\t}\r\n\t\t\tactivity = new TaskActivity(actTeacherId.join(','),actTeacherName.join(','),\"86727(A016090)\",\"云服务案例分析(A016090-线上实时、非实时结合授课)\",\"453\",\"信息A101(浑南校区)\",\"00000110000000000000000000000000000000000000000000000\",null,null,assistantName,\"\",\"\");\r\n\t\t\tindex =3*unitCount+6;\r\n\t\t\ttable0.activities[index][table0.activities[index].length]=activity;\r\n\t\t\tindex =3*unitCount+7;\r\n\t\t\ttable0.activities[index][table0.activities[index].length]=activity;\r\n\t\tvar teachers = [{id:10545,name:\"凌棕\",lab:false}];\r\n\t\tvar actTeachers = [{id:10545,name:\"凌棕\",lab:false}];\r\n\t\tvar assistant = _.filter(actTeachers, function(actTeacher) {\r\n\t\t\treturn (_.where(teachers, {id:actTeacher.id,name:actTeacher.name,lab:actTeacher.lab}).length == 0) && (actTeacher.lab == true);\r\n\t\t});\r\n\t\tvar assistantName = \"\";\r\n\t\tif (assistant.length > 0) {\r\n\t\t\tassistantName = assistant[0].name;\r\n\t\t\tactTeachers = _.reject(actTeachers, function(actTeacher) {\r\n\t\t\t\treturn _.where(assistant, {id:actTeacher.id}).length > 0;\r\n\t\t\t});\r\n\t\t}\r\n\t\tvar actTeacherId = [];\r\n\t\tvar actTeacherName = [];\r\n\t\tfor (var i = 0; i < actTeachers.length; i++) {\r\n\t\t\tactTeacherId.push(actTeachers[i].id);\r\n\t\t\tactTeacherName.push(actTeachers[i].name);\r\n\t\t}\r\n\t\t\tactivity = new TaskActivity(actTeacherId.join(','),actTeacherName.join(','),\"86727(A016090)\",\"云服务案例分析(A016090-线上实时、非实时结合授课)\",\"453\",\"信息A101(浑南校区)\",\"00000110000000000000000000000000000000000000000000000\",null,null,assistantName,\"\",\"\");\r\n\t\t\tindex =0*unitCount+6;\r\n\t\t\ttable0.activities[index][table0.activities[index].length]=activity;\r\n\t\t\tindex =0*unitCount+7;\r\n\t\t\ttable0.activities[index][table0.activities[index].length]=activity;\r\n\t\tvar teachers = [{id:10545,name:\"凌棕\",lab:false}];\r\n\t\tvar actTeachers = [{id:10545,name:\"凌棕\",lab:false}];\r\n\t\tvar assistant = _.filter(actTeachers, function(actTeacher) {\r\n\t\t\treturn (_.where(teachers, {id:actTeacher.id,name:actTeacher.name,lab:actTeacher.lab}).length == 0) && (actTeacher.lab == true);\r\n\t\t});\r\n\t\tvar assistantName = \"\";\r\n\t\tif (assistant.length > 0) {\r\n\t\t\tassistantName = assistant[0].name;\r\n\t\t\tactTeachers = _.reject(actTeachers, function(actTeacher) {\r\n\t\t\t\treturn _.where(assistant, {id:actTeacher.id}).length > 0;\r\n\t\t\t});\r\n\t\t}\r\n\t\tvar actTeacherId = [];\r\n\t\tvar actTeacherName = [];\r\n\t\tfor (var i = 0; i < actTeachers.length; i++) {\r\n\t\t\tactTeacherId.push(actTeachers[i].id);\r\n\t\t\tactTeacherName.push(actTeachers[i].name);\r\n\t\t}\r\n\t\t\tactivity = new TaskActivity(actTeacherId.join(','),actTeacherName.join(','),\"86727(A016090)\",\"云服务案例分析(A016090-线上实时、非实时结合授课)\",\"453\",\"信息A101(浑南校区)\",\"00001100000000000000000000000000000000000000000000000\",null,null,assistantName,\"\",\"\");\r\n\t\t\tindex =5*unitCount+0;\r\n\t\t\ttable0.activities[index][table0.activities[index].length]=activity;\r\n\t\t\tindex =5*unitCount+1;\r\n\t\t\ttable0.activities[index][table0.activities[index].length]=activity;\r\n\t\t\tindex =5*unitCount+2;\r\n\t\t\ttable0.activities[index][table0.activities[index].length]=activity;\r\n\t\t\tindex =5*unitCount+3;\r\n\t\t\ttable0.activities[index][table0.activities[index].length]=activity;\r\n\t\t\tindex =5*unitCount+4;\r\n\t\t\ttable0.activities[index][table0.activities[index].length]=activity;\r\n\t\t\tindex =5*unitCount+5;\r\n\t\t\ttable0.activities[index][table0.activities[index].length]=activity;\r\n\t\t\tindex =5*unitCount+6;\r\n\t\t\ttable0.activities[index][table0.activities[index].length]=activity;\r\n\t\t\tindex =5*unitCount+7;\r\n\t\t\ttable0.activities[index][table0.activities[index].length]=activity;\r\n\t\tvar teachers = [{id:10545,name:\"凌棕\",lab:false}];\r\n\t\tvar actTeachers = [{id:10545,name:\"凌棕\",lab:false}];\r\n\t\tvar assistant = _.filter(actTeachers, function(actTeacher) {\r\n\t\t\treturn (_.where(teachers, {id:actTeacher.id,name:actTeacher.name,lab:actTeacher.lab}).length == 0) && (actTeacher.lab == true);\r\n\t\t});\r\n\t\tvar assistantName = \"\";\r\n\t\tif (assistant.length > 0) {\r\n\t\t\tassistantName = assistant[0].name;\r\n\t\t\tactTeachers = _.reject(actTeachers, function(actTeacher) {\r\n\t\t\t\treturn _.where(assistant, {id:actTeacher.id}).length > 0;\r\n\t\t\t});\r\n\t\t}\r\n\t\tvar actTeacherId = [];\r\n\t\tvar actTeacherName = [];\r\n\t\tfor (var i = 0; i < actTeachers.length; i++) {\r\n\t\t\tactTeacherId.push(actTeachers[i].id);\r\n\t\t\tactTeacherName.push(actTeachers[i].name);\r\n\t\t}\r\n\t\t\tactivity = new TaskActivity(actTeacherId.join(','),actTeacherName.join(','),\"86727(A016090)\",\"云服务案例分析(A016090-线上实时、非实时结合授课)\",\"453\",\"信息A101(浑南校区)\",\"00000100000000000000000000000000000000000000000000000\",null,null,assistantName,\"\",\"\");\r\n\t\t\tindex =6*unitCount+0;\r\n\t\t\ttable0.activities[index][table0.activities[index].length]=activity;\r\n\t\t\tindex =6*unitCount+1;\r\n\t\t\ttable0.activities[index][table0.activities[index].length]=activity;\r\n\t\t\tindex =6*unitCount+2;\r\n\t\t\ttable0.activities[index][table0.activities[index].length]=activity;\r\n\t\t\tindex =6*unitCount+3;\r\n\t\t\ttable0.activities[index][table0.activities[index].length]=activity;\r\n\t\t\tindex =6*unitCount+4;\r\n\t\t\ttable0.activities[index][table0.activities[index].length]=activity;\r\n\t\t\tindex =6*unitCount+5;\r\n\t\t\ttable0.activities[index][table0.activities[index].length]=activity;\r\n\t\t\tindex =6*unitCount+6;\r\n\t\t\ttable0.activities[index][table0.activities[index].length]=activity;\r\n\t\t\tindex =6*unitCount+7;\r\n\t\t\ttable0.activities[index][table0.activities[index].length]=activity;\r\n\ttable0.marshalTable(2,1,18);\r\n\tfillTable(table0,7,12,0);\r\n</script>\r\n      \r\n  <div id='tasklesson' style=\"position: relative;\">\r\n          <br>\r\n \r\n<strong>课程列表:</strong>\r\n<div class=\"grid\">\r\n\r\n\r\n<table id=\"grid12042826911\" class=\"gridtable\">\r\n<thead class=\"gridhead\">\r\n\r\n\r\n<tr>\r\n<th  width=\"10%\" >序号</th>\r\n<th  width=\"15%\" >课程代码</th>\r\n<th  width=\"15%\" >课程名称</th>\r\n<th  width=\"10%\" >学分</th>\r\n<th  width=\"10%\" >课程序号</th>\r\n<th  width=\"15%\" >教师</th>\r\n<th  width=\"15%\" >备注</th>\r\n<th  width=\"10%\" >操作</th>\r\n</tr>\r\n\r\n</thead>\r\n\r\n<tbody id=\"grid12042826911_data\"><tr>\t\t<td>1</td>\r\n<td>A0800030010</td><td>全球视野下的人类文明与科技发展</td><td>1</td><td>\t\t<a href=\"/eams/courseTableForStd!taskTable.action?lesson.id=313931\" onclick=\"return bg.Go(this,null)\" title=\"点击显示单个教学任务具体安排\">A019378-线上实时授课</a>\r\n</td><td>李正鸿,陈红兵</td><td>      \t\t\t课程QQ群：1056439252\r\n</td><td>\t\t\t<a href=\"/eams/courseTableForStd!teachPlanInfo.action?lesson.id=313931\" target=\"_blank\">授课计划</a>\r\n\t\t\t<a href=\"/eams/courseTableForStd!courseSummaryInfo.action?lesson.id=313931\" target=\"_blank\">课程小结</a>\r\n</td></tr><tr>\t\t<td>2</td>\r\n<td>C0801003201</td><td>人机交互的软件工程方法</td><td>2</td><td>\t\t<a href=\"/eams/courseTableForStd!taskTable.action?lesson.id=311357\" onclick=\"return bg.Go(this,null)\" title=\"点击显示单个教学任务具体安排\">A016810-线上非实时授课</a>\r\n</td><td>赵建喆</td><td>      \t\t\t\r\n      \t\t\t<br/>\r\n      \t\t\t课程QQ群：1055285960\r\n</td><td>\t\t\t<a href=\"/eams/courseTableForStd!teachPlanInfo.action?lesson.id=311357\" target=\"_blank\">授课计划</a>\r\n\t\t\t<a href=\"/eams/courseTableForStd!courseSummaryInfo.action?lesson.id=311357\" target=\"_blank\">课程小结</a>\r\n</td></tr><tr>\t\t<td>3</td>\r\n<td>C0801006030</td><td>服务工程方法论</td><td>2</td><td>\t\t<a href=\"/eams/courseTableForStd!taskTable.action?lesson.id=311065\" onclick=\"return bg.Go(this,null)\" title=\"点击显示单个教学任务具体安排\">A016518-线上实时、非实时结合授课</a>\r\n</td><td>费园园</td><td>      \t\t\t课程QQ群：1055840456\r\n      \t\t\t<br/>\r\n      \t\t\t\r\n</td><td>\t\t\t<a href=\"/eams/courseTableForStd!teachPlanInfo.action?lesson.id=311065\" target=\"_blank\">授课计划</a>\r\n\t\t\t<a href=\"/eams/courseTableForStd!courseSummaryInfo.action?lesson.id=311065\" target=\"_blank\">课程小结</a>\r\n</td></tr><tr>\t\t<td>4</td>\r\n<td>C0801006041</td><td>云服务案例分析</td><td>2</td><td>\t\t<a href=\"/eams/courseTableForStd!taskTable.action?lesson.id=310637\" onclick=\"return bg.Go(this,null)\" title=\"点击显示单个教学任务具体安排\">A016090-线上实时、非实时结合授课</a>\r\n</td><td>凌棕</td><td>      \t\t\t教学办张老师QQ：282867398\r\n      \t\t\t<br/>\r\n      \t\t\t\r\n      \t\t\t<br/>\r\n      \t\t\t\r\n      \t\t\t<br/>\r\n      \t\t\t\r\n</td><td>\t\t\t<a href=\"/eams/courseTableForStd!teachPlanInfo.action?lesson.id=310637\" target=\"_blank\">授课计划</a>\r\n\t\t\t<a href=\"/eams/courseTableForStd!courseSummaryInfo.action?lesson.id=310637\" target=\"_blank\">课程小结</a>\r\n</td></tr><tr>\t\t<td>5</td>\r\n<td>C0801207060</td><td>人机交互程序设计实践</td><td>2</td><td>\t\t<a href=\"/eams/courseTableForStd!taskTable.action?lesson.id=310630\" onclick=\"return bg.Go(this,null)\" title=\"点击显示单个教学任务具体安排\">A016083</a>\r\n</td><td>吴辰铌,于洪超,于鲲鹏</td><td></td><td>\t\t\t<a href=\"/eams/courseTableForStd!teachPlanInfo.action?lesson.id=310630\" target=\"_blank\">授课计划</a>\r\n\t\t\t<a href=\"/eams/courseTableForStd!courseSummaryInfo.action?lesson.id=310630\" target=\"_blank\">课程小结</a>\r\n</td></tr></tbody>\r\n</table>\r\n</div>\r\n<script type=\"text/javascript\">\r\n  var clearCheckbox_grid12042826911 = function() {\r\n    jQuery(\"#grid12042826911\").find(\".box:checkbox\").removeProp(\"checked\");\r\n    jQuery(\"#grid12042826911\").find(\".gridselect-top :checkbox\").removeProp(\"checked\");\r\n    return true;\r\n  }\r\n  \r\n  page_grid12042826911 = bg.page(\"/eams/courseTableForStd!courseTable.action\",\"\");\r\n  {\r\n    var _paramstring = 'ignoreHead=1&showPrintAndExport=1&setting.kind=std&startWeek=&project.id=1&semester.id=31&ids=14625';\r\n    var _params = _paramstring.split('&');\r\n    for (var i = 0; i < _params.length; i++) {\r\n      _params[i] = decodeURIComponent(_params[i]);\r\n    }\r\n    _paramstring = _params.join('&');\r\n    page_grid12042826911.target(\"\",'grid12042826911').action(\"/eams/courseTableForStd!courseTable.action\").addParams(_paramstring).orderBy(\"null\");\r\n  }\r\n  bg.ui.grid.init('grid12042826911',page_grid12042826911);\r\n</script>\r\n  </div >   \r\n      \r\n      <br>\r\n          \r\n      <br>\r\n</div>\r\n\r\n</form>\r\n";
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var httpClientFactoryMock = mockHttpClientFactory.Object;
            var preferenceStorageProviderMock = new Mock<IPreferenceStorageProvider>();
            var mockPreferenceStorageProvider = preferenceStorageProviderMock.Object;
            var academicCalendar = new AcademicCalendar(mockPreferenceStorageProvider);
            var neuSyllabusGetter = new NeuSyllabusGetter(httpClientFactoryMock, academicCalendar);
            preferenceStorageProviderMock.Setup(p => p.Get("Campus", (int) Campus.Hunnan))
                .Returns((int) Campus.Hunnan);
            preferenceStorageProviderMock.Setup(p => p.Get("BaseDate", DateTime.MinValue))
                .Returns(new DateTime(2020, 2, 16));
            var neuEventList = neuSyllabusGetter.Parse(responseBody).OrderBy(x => x.Time).ToList();
            Assert.AreEqual(neuEventList.Count, 35);
            Assert.AreEqual(neuEventList[0].Time, new DateTime(2020, 2, 24, 8, 30, 0));
        }
    }
}