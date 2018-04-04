using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text;

[System.Serializable]
public class cell
{
    public Text label;
    public int solution;
    public bool clue;//tishi
    public int gridIndex;
    public int index;// 1 - 81
    public int horizontal;// 1 - 9
    public int vertical;
    public int box;



    #region 
    public void SubPosition()
    {
        int x = index / 9;
        if (index % 9 == 0)
        {
            horizontal = x;
        }
        else
        {
            horizontal = x + 1;
        }
        int y = index % 9;
        if (index % 9 == 0)
        {
            vertical = 9;
        }
        else
        {
            vertical = y;
        }
        if (horizontal >= 1 && horizontal <= 3)
        {
            if (vertical >= 1 && vertical <= 3)
            {
                box = 1;
            }
            else if (vertical >= 4 && vertical <= 6)
            {
                box = 2;
            }
            else if (vertical >= 7 && vertical <= 9)
            {
                box = 3;
            }
        }
        else if (horizontal >= 4 && horizontal <= 6)
        {
            if (vertical >= 1 && vertical <= 3)
            {
                box = 4;
            }
            else if (vertical >= 4 && vertical <= 6)
            {
                box = 5;
            }
            else if (vertical >= 7 && vertical <= 9)
            {
                box = 6;
            }
        }
        else if (horizontal >= 7 && horizontal <= 9)
        {
            if (vertical >= 1 && vertical <= 3)
            {
                box = 7;
            }
            else if (vertical >= 4 && vertical <= 6)
            {
                box = 8;
            }
            else if (vertical >= 7 && vertical <= 9)
            {
                box = 9;
            }
        }

    }
    #endregion
}

public class Sudoku : MonoBehaviour
{

    public enum clueGenerator
    {
        simple,
        efficient,
        automatic,
    }

    //hide these variables in the inspector (we're using a custom inspector)
    [HideInInspector]
    public clueGenerator clueGeneratorType;
    [HideInInspector]
    public float effectTime;
    [HideInInspector]
    public Color emptyColor;
    [HideInInspector]
    public Color labelColor;
    [HideInInspector]
    public Color clueColor;
    [HideInInspector]
    public Color clueLabelColor;
    [HideInInspector]
    public Color correctColor;
    [HideInInspector]
    public Color wrongColor;
    [HideInInspector]
    public float fadeSpeed;
    [HideInInspector]
    public int simpleClueGeneratorMinClues;

    //visible in the inspector
    public GameObject cell;
    public GameObject cellGrid;
    public GameObject numbers;
    public Image newButton;
    public Sprite cellBackground;


    public Image maxCluesHandle;

    public int currentLevel = 0;

    //not visible in the inspector

    int clueAttempts;

    GridLayoutGroup grid;
    RectTransform rect;

    bool fade;
    int solutionCount;

    [HideInInspector]
    public List<cell> cells = new List<cell>();
    List<int> notClues = new List<int>();

    void Start()
    {
        //PlayerPrefs.DeleteAll ();
        //don't yet show the numbers
        numbers.SetActive(false);

        //give the 'new' button a transparent color until the puzzle has been generated
        newButton.color = new Color(newButton.color.r, newButton.color.g, newButton.color.b, 0.3f);

        //get the grid and transform components
        grid = GetComponent<GridLayoutGroup>();
        rect = GetComponent<RectTransform>();

        //find the cell size and assign it to the grid
        float cellWidth = rect.rect.width / 3 - (2 * grid.spacing.x / 3) - (((float)grid.padding.left + (float)grid.padding.right) / 3);
        grid.cellSize = new Vector2(cellWidth, cellWidth);

        //get the last clues slider value if it wasn't 0 (0 would mean the player prefs were deleted)

        //get the start slider color based on the slider value
        maxCluesHandle.color = new Color((100f - (float)50) / 100f, (float)50 / 130f, 0.2f, 1);

        //if we're using the automatic generator, use the max amount of clues to find out whether we should use the simple or the efficient way of generating clues
        if (clueGeneratorType == clueGenerator.automatic)
        {
            if (50 < simpleClueGeneratorMinClues)
            {
                clueGeneratorType = clueGenerator.efficient;
            }
            else
            {
                clueGeneratorType = clueGenerator.simple;
            }
        }

        //start generating the puzzle
        generate();
    }

    void Update()
    {
        //if the puzzle has been generated and we're fading in the grid
        if (fade)
        {
            //change the button alpha and hide the loading text
            newButton.color = new Color(newButton.color.r, newButton.color.g, newButton.color.b, 1f);

            //show the numbers we can choose from
            if (!numbers.activeSelf)
                StartCoroutine(showNumbers());

            //if the cells are not fully visible yet
            if (cells[0].label.transform.parent.GetChild(1).GetComponent<Image>().color.a > 0)
            {
                //fade out all cell cover objects
                for (int i = 0; i < 81; i++)
                {
                    Image cellCover = cells[i].label.transform.parent.GetChild(1).GetComponent<Image>();
                    cellCover.color = new Color(cellCover.color.r, cellCover.color.g, cellCover.color.b, cellCover.color.a - (Time.deltaTime * fadeSpeed));
                }
            }
            else
            {
                //stop fading
                fade = false;
            }
        }
    }

    IEnumerator showNumbers()
    {
        //disable all numbers
        foreach (Transform child in numbers.transform)
        {
            child.gameObject.SetActive(false);
        }

        //enable the parent object
        numbers.SetActive(true);

        //for each number, enable it and wait a moment
        for (int i = 1; i <= 10; i++)
        {
            numbers.transform.GetChild(i).gameObject.SetActive(true);
            yield return new WaitForSeconds(effectTime / 15);
        }

        //wait 0.5 seconds and enable the first child object
        yield return new WaitForSeconds(0.5f);
        numbers.transform.GetChild(0).gameObject.SetActive(true);
    }

    void LoadANSWER()
    {
        char[] answers = Generator.getData();
        string levelstring = AnswerChcek.GetLevel();
        answers = levelstring.ToCharArray();
        Debug.Log(levelstring);
        //Solver Solver = new Solver();
        //Solver.load(answers);
        //Solver.dfs(0);
        //answers = Solver.getResult().ToCharArray();
        answers = TransAnswer(answers);
        for (int i = 0; i < 81; i++)
        {

            if (answers[i] != '.')
            {
                cells[i].solution = int.Parse(answers[i].ToString());
                cells[i].clue = true;


            }
            cells[i].SubPosition();
        }
        Debug.Log("是否有答案:" + AnswerChcek.CheckAnswerIsRight(cells));

    }

    char[] TransAnswer(char[] answer)
    {
        int k = 0;
        char[] answers = new char[81];
        for (int i = 0; i < 81; i++)
        {
            answers[i] = answer[MWC.trans[i] - 1];
            cells[i].index = MWC.trans[i];
        }


        return answers;
    }
    public void generate()
    {
        //first, add all empty cells
        for (int i = 0; i < 81; i++)
        {
            cells.Add(new cell { solution = 0 });
        }
        LoadANSWER();
        StartCoroutine(loadGrid());

    }



    IEnumerator loadGrid()
    {
        //for each sub-grid (there's 9 small grids with 9 cells each, part of one big grid)
        for (int i = 0; i < 9; i++)
        {
            //create the sub-grid and parent it to this main grid
            GameObject newGrid = Instantiate(cellGrid);
            newGrid.transform.SetParent(transform, false);

            //get the grid and transform components
            GridLayoutGroup newGridLayout = newGrid.GetComponent<GridLayoutGroup>();
            RectTransform newRect = newGrid.GetComponent<RectTransform>();

            //for all cells in this sub-grid
            for (int I = i * 9; I < i * 9 + 9; I++)
            {
                //wait a short moment and create the cell
                yield return new WaitForSeconds(effectTime / 81);
                GameObject newCell = Instantiate(cell);

                //get the cell label
                //cells[I].label = newCell.transform.GetChild(0).GetComponent<Text>();

                //parent the cell to the sub-grid and name it after its index
                newCell.transform.SetParent(newGrid.transform, false);
                newCell.name = I + "";
                newCell.GetComponent<Cell>().Init(cells[I]);
                //get the cell size and assign it to the grid
                float cellWidth = newRect.rect.width / 3 - (2 * newGridLayout.spacing.x / 3) - (((float)newGridLayout.padding.left + (float)newGridLayout.padding.right) / 3);
                newGridLayout.cellSize = new Vector2(cellWidth, cellWidth);
            }
        }

        //wait a moment and get the clues
        yield return new WaitForSeconds(0.2f);
        getClues();
    }


    public void getClues()
    {

        for (int N = 0; N < 81; N++)
        {
            //get the cell image
            Image image = cells[N].label.transform.parent.GetComponent<Image>();

            //if this cell is a clue
            if (cells[N].clue)
            {
                //show the solution, change the cell colors and set sprite to null
                cells[N].label.text = cells[N].solution + "";
                cells[N].label.color = clueLabelColor;
                image.color = clueColor;
                image.sprite = null;
            }
            else
            {
                //if this is not a clue, don't show the solution yet and show the cell background
                cells[N].label.text = "";
                cells[N].label.color = labelColor;
                image.color = emptyColor;
                image.sprite = cellBackground;
            }
        }

        //start fading the grid
        fade = true;
    }


    public void restart()
    {
        //if we can load a new puzzle, restart the current scene
        if (newButton.color.a == 1)
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public string GetCurrentLevels()
    {

        int level = PlayerPrefs.GetInt("CurrentLevel", 1);
        string levelstring = "";
        if (currentLevel == level)
        {
            levelstring = PlayerPrefs.GetString("LevelProgress", "");
        }
        else
        {
            levelstring = AnswerChcek.GetLevel();
        }
        return levelstring;
    }

    public void CheckWin()
    {
        bool win = AnswerChcek.CheckAnswerIsRight(cells);
        if(win)Debug.Log("Game Win");
    }

    public void OnDraged(cell cel)
    {
       bool itRight = AnswerChcek.CheckCellIsRight(cells,cel);
        if (!itRight)
        {
            cel.label.color = Color.red;
        }
        else
        {
            cel.label.color = Color.green;
        }
        CheckWin();
    }
}