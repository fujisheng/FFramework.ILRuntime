using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    string testStr = @"
using UnityEngine;
public class TestClass
{
public void Test(){
UnityEngine.Debug.Log(""Test"");}
}
";
    // Start is called before the first frame update
    void Start()
    {
        var assembly = CSharpScriptEngine.GetAssembly(new string[] { testStr });
        var testClass = assembly.CreateInstance("TestClass");
        testClass.GetType().GetMethod("Test").Invoke(testClass, null);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
