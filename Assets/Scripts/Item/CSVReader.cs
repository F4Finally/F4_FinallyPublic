using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class CSVReader
{
    //특수문자가 들어갈 수 있기 때문에 변환해주는 부분
    static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";  
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    static char[] TRIM_CHARS = { '\"' };

    public static List<Dictionary<string, string>> Read(string file) //리스트 안에 한줄한줄 딕셔너리<스트링,오브젝트> 형태로 만들어서 넣어준다
                                                                     //원래 <string, object> 였는데 박싱언박싱 어쩌구 해서 <string,string> 으로 바꿔줬음
    {
        var list = new List<Dictionary<string, string>>();
        TextAsset data = Resources.Load(file) as TextAsset; //CSV 파일을 Resources 폴더에서 불러온다! 그 폴더에 있어야 함

        var lines = Regex.Split(data.text, LINE_SPLIT_RE);

        if (lines.Length <= 1) return list; 

        var header = Regex.Split(lines[0], SPLIT_RE);
        for (var i = 1; i < lines.Length; i++)
        {

            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "") continue;

            var entry = new Dictionary<string, string>();
            for (var j = 0; j < header.Length && j < values.Length; j++) //각 줄에 해당 정보 넣는걸 줄 수만큼 반복하기
            {
                string value = values[j];
                value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
               
                entry[header[j]] =value;
            }
            list.Add(entry);
        }
        return list;
    }
}

