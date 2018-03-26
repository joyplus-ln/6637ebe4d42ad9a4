
using System;
using System.IO;
using UnityEngine;

public class Generator
{
    public static char[] data = new char[81];
    private char[] tmp = new char[81];
    private char[] map = { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
    private Solver solver;

    private Generator()
    {
        solver = new Solver();
    }

    private void randomFill()
    {
        int x;
        MWC.setSeed((int)GetCurrentTimeUnix());
        for (int i = 0; i < 81; i++)
        {
            data[i] = '.';
        }
        for (int i = 0; i < 9; i++)
        {
            do
                x = MWC.random() >> 9 & 15;
            while (x > 8);
            data[i * 9 + x] = map[i];
        }
        solver.load(data);
        solver.dfs(0);
    }
    public long GetCurrentTimeUnix()
    {
        TimeSpan cha = (DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)));
        long t = (long)cha.TotalSeconds;
        return t;
    }
    private bool[] hasTryed = new bool[81];

    private void digHole()
    {
        char ch;
        int i, j, k, m;
        for (i = 0; i < 81; i++)
            hasTryed[i] = false;
        bool flag;
        for (i = 0; i < 81; i++)
        {
            do
                m = MWC.random() >> 8 & 127;
            while (m >= 81 || hasTryed[m]);
            hasTryed[m] = true;
            ch = data[m];
            flag = false;
            for (j = 0; j < 9; j++)
                if (ch != map[j])
                {
                    for (k = 0; k < 81; k++)
                        tmp[k] = data[k];
                    tmp[m] = map[j];
                    solver.load(tmp);
                    solver.dfs(0);
                    if (solver.hasResult())
                    {
                        flag = true;
                        break;
                    }
                }
            if (!flag)
                data[m] = '.';
            else
                data[m] = ch;
        }
    }

    private static Generator generator = null;

    public static String getSudoke()
    {
        if (generator == null)
        {
            generator = new Generator();
        }
        generator.randomFill();
        generator.digHole();
        
        return new string(data);
    }

    public static char[] getData()
    {
        if (generator == null)
        {
            generator = new Generator();
        }
        generator.randomFill();
        generator.digHole();
        return data;
    }

    public static void main(String[] args)
    {
        // Generator g = new Generator();
        // g.randomFill();
        // System.out.println(g.data);
        // g.digHole();
        // System.out.println(g.data);
        //
        // Solver solver = new Solver();
        // solver.load(g.data);
        // solver.dfs(0);
        // if (solver.hasResult()) {
        // System.out.println(g.data);
        // }
        Debug.Log(Generator.getSudoke());

    }
}
