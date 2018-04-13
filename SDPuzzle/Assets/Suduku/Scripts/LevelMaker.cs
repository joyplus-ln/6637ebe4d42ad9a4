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
        string name = Application.dataPath + "/" + string.Format("Levels{0}.txt", "SD");
        // File.Create(name);
        FileStream fs = new FileStream(name, FileMode.Append, FileAccess.Write);
        StreamWriter sw = new StreamWriter(fs);
        int level = 5;
        int time = 0;

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
            time++;
            if (time > 15)
            {
                time = 0;
                level++;
            }
            data = HandleLevelWithDot(data, level);
            sw.WriteLine(data);
            sw.Write(',');
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

    static char[] HandleLevelWithDot(char[] data, int levels)
    {
        System.Random random = new System.Random();
        for (int i = 0; i < levels; i++)
        {
            data[random.Next(data.Length - 1)] = '.';
        }
        return data;
    }
}
