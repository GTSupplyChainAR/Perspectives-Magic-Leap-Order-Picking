using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

public class BookInfoView : MonoBehaviour
{

    private GameObject bookText;
    private GameObject book;
    private int maxLineChars = 5;
    private String[] words;
    String result = "";
    private int charCount;
    private int shelfHighlightNumber;
    private int highlight_row;
    private Dictionary<string, int> row;

    // Use this for initialization
    public void init() {
        row = new Dictionary<string, int>();
        row.Add("A", 0);
        row.Add("B", 1);
        row.Add("C", 2);
        row.Add("D", 3);
        row.Add("E", 4);
        row.Add("F", 5);
        highlight_row = -1;
        bookText = GameObject.Find("Book Text");
    }
   
    // Use this for highlighting book
    public void highlightBookInfo(BookWithLocation bookInfo)
    {
        highlightRow(bookInfo.book.tag);
        highlightBook(bookInfo);
    }
    private void highlightBook(BookWithLocation bookInfo) {
        TextMesh bookInfoText = bookText.GetComponent<TextMesh>();
        bookInfoText.text = "Title: ";
        wrapText(bookInfo.book.title);
        bookInfoText.text = bookInfoText.text + "\nAuthor: ";
        wrapText(bookInfo.book.author);
    }
    private void highlightRow(String tag)
    {
        //Get rid of old highlight
        String objectID;
        if (highlight_row != -1)
        {
            objectID = "row_" + highlight_row + "_block";
            Sprite greenBlock = Resources.Load<Sprite>("green_block");
            GameObject.Find(objectID).GetComponent<SpriteRenderer>().sprite = greenBlock;
        }

        //Highlight new block
        string[] loc = tag.Split('-');
        highlight_row = row[loc[3]];
        objectID = "row_" + highlight_row + "_block";
        Sprite redBlock = Resources.Load<Sprite>("red_block");
        GameObject.Find(objectID).GetComponent<SpriteRenderer>().sprite = redBlock;
        GameObject.Find("shelf_number_text").GetComponent<TextMesh>().text = loc[3];
    }
 

    void Start()
    {
        init();
    }
    void Update()
    {
        
    }

 

    private void wrapText(String text)
    {
        charCount = 0;
        words = text.Split(" "[0]);
        result = "";

        for (int index = 0; index < words.Length; index++)
        {

            var word = words[index].Trim();

            if (index == 0)
            {
                result = words[0];
            }

            if (index > 0)
            {
                charCount += word.Length + 1;
                if (charCount <= maxLineChars)
                {
                    result += " " + word;
                }
                else
                {
                    charCount = 0;
                    result += "\n " + word;
                }
            }
        }
        bookText.GetComponent<TextMesh>().text = bookText.GetComponent<TextMesh>().text + result;
    }
}
