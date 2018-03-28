using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class LevelMaker : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }
    [MenuItem("LevelMaker/MakeLevel")]
    public static void MakeLevels()
    {
        string name = Application.dataPath + "/" + string.Format("MakeLevels{0}.txt",777);
       // File.Create(name);
        FileStream fs = new FileStream(name, FileMode.Append, FileAccess.Write);
        StreamWriter sw = new StreamWriter(fs);
        
        
        LevelGenerator lgGenerator = new LevelGenerator();
        char[] data;
        char[] temp = new char[81];
        data = Generator.getData();
        lgGenerator.handle(data);

        Solver solver = new Solver();

        for (int i = 0; i < 500; i++)
        {
            data = Generator.getData();
            lgGenerator.handle(data);
            if (lgGenerator.isInvalid)
            {
                i--;
                continue;
            }
            for (int j = 0; j < 81; j++)
            {
                temp[j] = data[j];
            }

            solver.load(temp);
            solver.dfs(0);
            if (!solver.hasResult())
            {
                Debug.Log(temp.ToString() + " no result ");
                i--;
                continue;
            }
            Solver Solver = new Solver();
            Solver.load(data);
            Solver.dfs(0);
            data = Solver.getResult().ToCharArray();
            sw.WriteLine(data);

        }
        sw.Close();
        fs.Close();
    }

    [MenuItem("LevelMaker/CheckAnswer")]
    public static void CheckAnswer()
    {
        
    }

    static void CheckHasAnswer(string data)
    {
        
    }
}
