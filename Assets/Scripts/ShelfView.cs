using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class ShelfView : MonoBehaviour
{

    Sprite block;
    int highlight_row;
    int highlight_col;
    Dictionary<string, int> column;
    Dictionary<string, int> row;
    string[] block_names = new string[] {
        "red_block",
        "orange_block",
        "yellow_block",
        "green_block",
        "blue_block",
        "purple_block"
    };
    string[] empty_block_names = new string[] {
        "red_empty_block",
        "orange_empty_block",
        "yellow_empty_block",
        "green_empty_block",
        "blue_empty_block",
        "purple_empty_block"
    };
    public void init()
    {
        //Debug.Log("Init");
        highlight_row = -1;
        highlight_col = -1;

        column = new Dictionary<string, int>();
        row = new Dictionary<string, int>();
        column.Add("1", 0);
        column.Add("2", 1);
        column.Add("3", 2);

        row.Add("A", 0);
        row.Add("B", 1);
        row.Add("C", 2);
        row.Add("D", 3);
        row.Add("E", 4);
        row.Add("F", 5);
        row.Add("G", 6);

    }
    private void updateAisle(string text)
    {
        //GameObject.Find("aisle_text").GetComponent<TextMesh>().text = text;
    }
    private void updateColoumTexts(string[] col_texts)
    {
        int min = col_texts.Length > 3 ? 3 : col_texts.Length;
        for (int i = 0; i < min; i++)
        {
            GameObject.Find("column_text_" + i).GetComponent<TextMesh>().text = col_texts[i];
        }
    }
    private void highlightBlock(int row, int column)
    {
        
        if (row >= 0 && row < 7 && column >= 0 && column < 3)
        {
            Sprite block = Resources.Load<Sprite>(this.block_names[row]);
            GameObject.Find("empty_block_" + row + "_" + column).GetComponent<SpriteRenderer>().sprite = block;
            if(row == highlight_row && column == highlight_col)
            {
                // no action
            }else if (highlight_col != -1 && highlight_row != -1)
            {
                Sprite empty_block = Resources.Load<Sprite>(this.empty_block_names[highlight_row]);
                GameObject.Find("empty_block_" + highlight_row + "_" + highlight_col).GetComponent<SpriteRenderer>().sprite = empty_block;
            }
            highlight_col = column;
            highlight_row = row;
        }
    }
    // 
    public void highlightBlock(BookWithLocation bookInfo)
    {
        string tag = bookInfo.book.tag;
        string[] loc = tag.Split('-');
        
        int row_id = row[loc[0]];
        int col_id = column[loc[1]];
        string aisle = loc[0];
        
            highlightBlock(row_id, col_id);
            updateColoumTexts(new string[] { "1", "2", "3" });
            updateAisle(aisle);
       
    }
    void Start()
    {
        
        //init();
    }

    // Update is called once per frame
    void Update()
    {

    }

}
