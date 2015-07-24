using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoomGenerator
{
    public class TextBox
    {
        OnScreenText textToDisplay;
        string text = string.Empty;
        Vector2 m_Position = new Vector2(20, 100);
        Vector2 m_TextDimensions;

        Rectangle m_TextBox;

        float transparencyModifier = 0.85f;
        float fadeTimer = 0;
        float completeFade = 5;
        bool startFade = false;

        int rightMost;

        public TextBox(string a_Text)
        {
            rightMost = Utility.Instance.ScreenWidth - 100;

            Displayed = true;

            text = a_Text;

            m_TextDimensions = Utility.Instance.NormalTextFont.MeasureString(text);

            int totalLines = 0;

            string formattedSentence = FormatText(text, rightMost, ref totalLines);

            //takes into account the entire height of text box
            int finalHeightTextBox = (int)(m_TextDimensions.Y * (totalLines == 1 ? 1 : totalLines - 1));

            int positionPadding = 10;

            //position from bottom of screen using text box height, plus extra padding
            m_Position.Y = Utility.Instance.ScreenHeight - finalHeightTextBox - positionPadding;

            textToDisplay = new OnScreenText(formattedSentence, m_Position, Color.White, Color.RoyalBlue, Utility.Instance.NormalTextFont);
            textToDisplay.CompleteFade = completeFade;

            //also changes textToDisplay's variables
            StartFade = false;

            //hack to make text splitting work, add extra to rightMost boundary
            int rightBoundaryPadding = 60;

            m_TextBox = new Rectangle((int)m_Position.X, (int)m_Position.Y, rightMost + rightBoundaryPadding, finalHeightTextBox);
        }

        public void Update(float deltaTime)
        {
            if (Displayed)
            {
                if (StartFade)
                {
                    fadeTimer += deltaTime;
                }

                textToDisplay.Update(deltaTime);
                float diff = Math.Abs(completeFade - fadeTimer);
                if (diff >= 0 && diff <= 1)
                {
                    Displayed = false;
                    textToDisplay.Displayed = false;
                }
            }
        }

        public void Draw()
        {
            if (Displayed)
            {
                Utility.Instance.SB.Draw(Utility.Instance.WhiteSquare, m_TextBox, Color.Black * transparencyModifier * (1 - (fadeTimer / completeFade)));
                textToDisplay.Draw();
            }
        }

        string FormatText(string a_Text, int rightMost, ref int totalLines)
        {
            //foreach word in sentence
            //  foreach letter in word
            //      if adding on another letter to current sentence is greater than right most
            //          add new line first, then add next letters
            //first, split up the words
            string[] words = a_Text.Split(' ');
            string newSentence = string.Empty;
            string firstCopy = string.Empty;
            int numLines = 1;

            //loop through each word in the sentence
            foreach (string word in words)
            {
                //add word and space
                //or add new line, word and space
                bool addWord = true;

                //marks stopping point when letter in word goes off screen
                int breakingIndex = 0;

                //loop through each letter in the current word
                foreach (char letter in word)
                {
                    //move stopping point along
                    breakingIndex++;

                    //add letter to be measured
                    firstCopy += letter;

                    //measure length of current word
                    float currentWordLength = Utility.Instance.NormalTextFont.MeasureString(firstCopy).X;

                    //if the length of the current word is greater 
                    //than a multiple of the right most boundary
                    //then dont just add a word, also add a new line
                    if (currentWordLength > rightMost * numLines)
                    {
                        //if the break is not on the full length of the word
                        //then remove the letters added, and fill in the rest
                        if (breakingIndex != word.Length)
                        {
                            //remove from here to end of firstCopy string
                            int indexToStartRemoving = firstCopy.Length - breakingIndex;
                            firstCopy = firstCopy.Remove(indexToStartRemoving, breakingIndex);
                            firstCopy += word;
                        }

                        addWord = false;
                        numLines++;
                        break;
                    }
                }

                firstCopy += " ";

                if (addWord)
                {
                    newSentence += word + " ";
                }
                else
                {
                    newSentence += '\n' + word + " ";
                }
            }

            totalLines = numLines;

            return newSentence;
        }

        //origin is top left, need to account for final height of text box
        public void ChangeVerticalPosition(float vertPos)
        {
            textToDisplay.ChangePosition(new Vector2(m_Position.X, vertPos));
            m_TextBox.Y = (int)vertPos;
        }

        public void ChangeTextBoxTransparency(float transparency)
        {
            transparencyModifier = transparency;

            //keep final value between 0 - 1
            if (transparencyModifier > 1)
            {
                transparencyModifier = 1;
            }

            if (transparencyModifier < 0)
            {
                transparencyModifier = 0;
            }
        }

        public void ChangeDisplayedText(string a_Text)
        {
            if (a_Text.Length == 0)
            {
                return;
            }

            m_TextDimensions = Utility.Instance.NormalTextFont.MeasureString(a_Text);

            int totalLines = 0;
            string formatted = FormatText(a_Text, rightMost, ref totalLines);
            textToDisplay.ChangeText(formatted);

            //takes into account the entire height of text box
            int finalHeightTextBox = (int)(m_TextDimensions.Y * (totalLines == 1 ? 1 : totalLines - 1));

            int positionPadding = 10;

            //position from bottom of screen using text box height, plus extra padding
            m_Position.Y = Utility.Instance.ScreenHeight - finalHeightTextBox - positionPadding;

            //change displayed text position
            textToDisplay.ChangePosition(m_Position);

            //hack to make text splitting work, add extra to rightMost boundary
            int rightBoundaryPadding = 60;

            //change text box dimensions
            m_TextBox.X = (int)m_Position.X;
            m_TextBox.Y = (int)m_Position.Y;
            m_TextBox.Width = rightMost + rightBoundaryPadding;
            m_TextBox.Height = finalHeightTextBox;
        }

        public bool Displayed
        {
            get;
            set;
        }

        public bool StartFade
        {
            get
            {
                return startFade;
            }
            set
            {
                startFade = value;
                textToDisplay.StartFade = value;
            }
        }
    }
}
