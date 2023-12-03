//Author: Victoria Mak
//File Name: Game1.cs
//Project Name: HumanBenchmark
//Creation Date: April 27, 2022
//Modified Date: May 13, 2022
//Description: Collection of mini games (reaction game, aim trainer, and number memory) in the human benchmark game

using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Animation2D;
using Helper;

namespace HumanBenchmark
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        //Store the random number generator
        Random rng = new Random();

        //Game States
        const int MENU = 0;
        const int PREGAME = 1;
        const int GAME = 2;
        const int ENDGAME = 3;

        //Game Types
        const int NONE = -1;
        const int REACTION = 0;
        const int AIM = 1;
        const int NUMBER = 2;
        const int SEQUENCE = 3;

        //Hover Status
        const int OFF = 0;
        const int ON = 1;

        //UI Layout Offsets
        const int TITLE_OFFSET = 20;
        const int TITLE_SPACER = 10;
        const int MENU_TEXT_SPACER = 25;
        const int MENU_HORIZ_SPACER = 20;
        const int MENU_BTN_SPACER = 10;
        const int PRE_VERT_SPACER = 100;
        const int PRE_TEXT_SPACER = 50;
        const int END_VERT_SPACER = PRE_VERT_SPACER;
        const int END_TEXT_SPACER = PRE_TEXT_SPACER;
        const int END_BUTTON_SPACER = 30;
        const int REACT_TEXT_SPACER = TITLE_SPACER;

        //Store the number of targets for aim 
        const int MAX_TARGETS = 30;

        //Store the number of rounds for reaction
        const int REACT_MAX_ROUNDS = 5;

        //Store the min and max wait time for reaction in milliseconds
        const int MIN_WAIT_TIME = 1500;
        const int MAX_WAIT_TIME = 3500;

        //Store the reaction states
        const int RED = 0;
        const int GREEN = 1;
        const int TIME_RECORDED = 2;
        const int TOO_EARLY = 3;

        //Store the states in the number game
        const int MEMORIZING = 0;
        const int TYPING = 1;

        //Store the max level in number memory
        const int NUMER_MAX_LEVEL = 15;

        //Store the minimum memorizing time and the memorizing time increment by level in the number game in milliseconds
        const int MIN_MEMORIZING_TIME = 1700;
        const int MEMORIZING_TIME_INCREMENT = 800;

        //Store the element number for the time bar images
        const int EMPTY = 0;
        const int FULL = 1;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        //Store the reaction time background color
        Color waitColor = new Color(206, 38, 54);
        Color clickColor = new Color(75, 219, 106);

        //Store the right and wrong colors for the digits in the number game
        Color right = Color.White;
        Color wrong = Color.Black;

        //Game background color
        Color regularBgColor = new Color(43, 135, 209);
      
        //Store the current background color
        Color curBgColor;

        //Store the current game state and the game type chosen
        int gameState = MENU;
        int gameType = NONE;

        //Store the mouse and keyboard states
        MouseState mouse;
        MouseState prevMouse;
        KeyboardState kb;
        KeyboardState prevKb;

        //Store the screen width and height
        int screenWidth;
        int screenHeight;

        //Store the fonts for the title, instructions, menu, and numbers for the number game
        SpriteFont titleFont;
        SpriteFont instFont;
        SpriteFont menuFont;
        SpriteFont numberFont;

        //Store the reaction icon image and the buttons for the different games and the start, try and save buttons
        Texture2D reactionIconImg;
        Texture2D[] reactionBtns = new Texture2D[2];
        Texture2D[] aimBtns = new Texture2D[2];
        Texture2D[] numberBtns = new Texture2D[2];
        Texture2D[] sequenceBtns = new Texture2D[2];
        Texture2D[] startBtns = new Texture2D[2];
        Texture2D[] tryBtns = new Texture2D[2];
        Texture2D[] saveBtns = new Texture2D[2];

        //Store the different pre game icons
        Texture2D[] pregameImgs = new Texture2D[4];

        //Store the end of game icons
        Texture2D[] endgameImgs = new Texture2D[4];

        //Store the 3 different reaction icons
        Texture2D[] reactGameIconImgs = new Texture2D[4];

        //Store the aim target image
        Texture2D targetImg;
        
        //Store the progress bar image and text box image for the number game
        Texture2D[] timeBarImgs = new Texture2D[2];
        Texture2D textBoxImg;

        //Store the rectangles for the title icon and the game buttons in the menu
        Rectangle titleIconRec;
        Rectangle[] titleBtnRecs = new Rectangle[4];

        //Store the rectangles for the pregame icons
        Rectangle[] preIconRecs = new Rectangle[4];

        //Store the rectangles for the endgame icons
        Rectangle[] endgameIconRecs = new Rectangle[4];

        //Store the rectangle for the start, save, and try buttons
        Rectangle startRec;
        Rectangle saveRec;
        Rectangle tryRec;

        //Store the target rectangle
        Rectangle targetRec;

        //Store the 3 reaction game icon rectangles
        Rectangle[] reactGameIconRecs = new Rectangle[4];

        //Store the rectangle for the time bar and text box in the number memory game
        Rectangle[] timeBarRecs = new Rectangle[2];
        Rectangle textBoxRec;

        //Store the locations for the title and title description
        Vector2 titleLoc;
        Vector2 titleDescLoc;

        //Store the location for the name of the games in the menu
        Vector2 menuReactionLoc;
        Vector2 menuAimLoc;
        Vector2 menuNumberLoc;
        Vector2 menuSequenceLoc;

        //Store the location for the best scores for the games
        Vector2 menuReactionBestLoc;
        Vector2 menuAimBestLoc;
        Vector2 menuNumberBestLoc;
        Vector2 menuSequenceBestLoc;

        //Store the location for the pregame title and description
        Vector2 pregameTitleLoc;
        Vector2 pregameDescLoc;

        //Store the end game message locations
        Vector2[] endTitleLocs = new Vector2[4];
        Vector2 endScoreLoc;
        Vector2 endSaveScoreLoc;
        
        //Store the location for the remaining targets message
        Vector2 aimTgtRemainingLoc = new Vector2();

        //Store the location for the reaction display messages in the game
        Vector2[] reactGameMsgLocs = new Vector2[4];
        Vector2[] reactContinueLocs = new Vector2[4];

        //Store the location for the digits in the number game
        Vector2 digitLoc;

        //Store the location for the instructional prompts in the number game
        Vector2 digitQuestionLoc;
        Vector2 enterInstrLoc;

        //Store the location for the user's answer in the number game
        Vector2 userAnswerLoc;

        //Store the location for the number label, correct number, user's answer label and the user's digits in the end game screen for the number game
        Vector2 numLblLoc;
        Vector2 corNumLoc;
        Vector2 userAnsLblLoc;
        Vector2[] userDgtLocs = new Vector2[NUMER_MAX_LEVEL];

        //Store the title of the game and the description
        string title = "Human Benchmark";
        string titleDesc = "Measure your abilities with brain games and cognitive tests.";

        //Store the game names
        string menuReaction = "Reaction Time";
        string menuAim = "Aim Trainer";
        string menuNumber = "Number Memory";
        string menuSequence = "Sequence Memory";

        //Store the best scores for the games
        string menuReactionBest = "--- ms";
        string menuAimBest = "--- ms";
        string menuNumberBest = "--- pts";
        string menuSequenceBest = "--- pts";

        //Store the game titles and descriptions for the pregame screen
        string[] preTitle = new string[] { "Reaction Time Test",
                                           "Aim Trainer",
                                           "Number Memory",
                                           "Sequence Memory Test" };
        string[] preDesc = new string[] { "When the red box turns green, click as quickly as you can.",
                                          "Hit 30 targets as quickly as you can.",
                                          "The average person can remember 7 numbers at once. Can you do more?",
                                          "Memorize the pattern." };

        //Store the end of game title and score message
        string[] endTitles = new string[4];
        string scoreMsg = " ";

        string endSaveScoreMsg = "Save your score to see how you compare.";

        //Store the string for the number of targets remaining in aim
        string aimTargetsRemainingMsg;

        //Store the main reaction display messages and the message saying click to keep going or try again
        string[] reactGameMsgs = new string[] { "Wait for green",
                                                "Click!",
                                                " ms",
                                                "Too soon!" };
        string[] reactContinueMsgs = new string[] {"",
                                                   "",
                                                   "Click to keep going",
                                                   "Click to try again."};


        //Store the string for the digits in the number game
        string digitMsg = " ";

        //Store the instructional prompts for the user to enter the digits in the number game
        string digitQuestion = "What was the number?";
        string enterInstrMsg = "Press enter to submit";

        //Store the user typed number for the number game
        string userAnswer = " ";

        //Store the number label and the user's answer label in the end game screen for the number game
        string numberLabel = "Number";
        string userAnswerLabel = "Your answer";

        //Store whether the mouse is hovering over the game buttons in the menu screen
        bool hoverReactionBtn = false;
        bool hoverAimBtn = false;
        bool hoverNumberBtn = false;
        bool hoverSequenceBtn = false;

        //Store whether the mouse is hovering over the start, save, or try button
        bool startHoverBtn = false;
        bool saveHoverBtn = false;
        bool tryHoverBtn = false;

        //Store the target radius squared, and the mouse distance from the target center squared
        int sqrTgtRadius;
        double sqrDistFromTgtCntr;

        //Store the times for the aim game
        double[] aimTimes = new double[MAX_TARGETS] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        //Store the number of targets remaining
        int targetsRemaining = MAX_TARGETS;

        //Store the reaction times
        double[] reactionTimes = new double[5];

        //Store the reaction round, the reaction state, and the next waiting time
        int reactionRound = 1;
        int reactionState;
        double waitTime;

        //Store the number game state and the round number
        int numberState;
        int numberRound = 1;

        //Store the memorizing time in the number game
        int memorizingTime = MIN_MEMORIZING_TIME;

        //Store the sequence of the number game, and the array of whether the user's digits matches through the digit's color
        int[] numberDigits = new int[NUMER_MAX_LEVEL];
        Color[] userDgtColors = new Color[NUMER_MAX_LEVEL];

        //Store the percentage of the time bar
        float barPercentage = 1f;
        
        //Store the timer used for the aim game
        Timer targetTimer;

        //Store the wait timer and reaction timer used for the reaction game
        Timer waitTimer;
        Timer reactionTimer;

        //Store the memorizing timer in the reaction game
        Timer memorizingTimer;

        //Store the current game score
        int score;

        //Store the game high scores
        int aimBestScore = -1;
        int reactionBestScore = -1;
        int numberBestScore = -1;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            //Set the game screen size
            graphics.PreferredBackBufferWidth = 980;
            graphics.PreferredBackBufferHeight = 550;

            //Set the mouse as visible
            IsMouseVisible = true;

            //Apply the changes to the game graphics
            graphics.ApplyChanges();

            //Set the screen width and height
            screenWidth = graphics.GraphicsDevice.Viewport.Width;
            screenHeight = graphics.GraphicsDevice.Viewport.Height;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Set the current background color
            curBgColor = regularBgColor;

            //Load the fonts
            titleFont = Content.Load<SpriteFont>("Fonts/TitleFont");
            instFont = Content.Load<SpriteFont>("Fonts/InstFont");
            menuFont = Content.Load<SpriteFont>("Fonts/MenuFont");
            numberFont = Content.Load<SpriteFont>("Fonts/NumberFont");

            //Load the reaction icon
            reactionIconImg = Content.Load<Texture2D>("Images/Sprites/ReactionIconLg");

            //Load the game icons and set the reaction game icon as the human benchmark icon
            pregameImgs[REACTION] = reactionIconImg;
            pregameImgs[AIM] = Content.Load<Texture2D>("Images/Sprites/Target");
            pregameImgs[NUMBER] = Content.Load<Texture2D>("Images/Sprites/NumberIconLg");
            pregameImgs[SEQUENCE] = Content.Load<Texture2D>("Images/Sprites/SequenceIconLg");

            //Load button images
            reactionBtns[OFF] = Content.Load<Texture2D>("Images/Sprites/ReactionBtn");
            reactionBtns[ON] = Content.Load<Texture2D>("Images/Sprites/ReactionHoverBtn");
            aimBtns[OFF] = Content.Load<Texture2D>("Images/Sprites/AimBtn");
            aimBtns[ON] = Content.Load<Texture2D>("Images/Sprites/AimHoverBtn");
            numberBtns[OFF] = Content.Load<Texture2D>("Images/Sprites/NumberBtn");
            numberBtns[ON] = Content.Load<Texture2D>("Images/Sprites/NumberHoverBtn");
            sequenceBtns[OFF] = Content.Load<Texture2D>("Images/Sprites/SequenceBtn");
            sequenceBtns[ON] = Content.Load<Texture2D>("Images/Sprites/SequenceHoverBtn");
            startBtns[OFF] = Content.Load<Texture2D>("Images/Sprites/StartBtn");
            startBtns[ON] = Content.Load<Texture2D>("Images/Sprites/StartHoverBtn");
            tryBtns[OFF] = Content.Load<Texture2D>("Images/Sprites/TryBtn");
            tryBtns[ON] = Content.Load<Texture2D>("Images/Sprites/TryHoverBtn");
            saveBtns[OFF] = Content.Load<Texture2D>("Images/Sprites/SaveBtn");
            saveBtns[ON] = Content.Load<Texture2D>("Images/Sprites/SaveHoverBtn");

            //Set the target image for the aim game
            targetImg = pregameImgs[AIM];

            //Load the reaction icon images
            reactGameIconImgs[RED] = Content.Load<Texture2D>("Images/Sprites/ReactionWaitIcon");
            reactGameIconImgs[TOO_EARLY] = Content.Load<Texture2D>("Images/Sprites/OopsIcon");
            reactGameIconImgs[TIME_RECORDED] = Content.Load<Texture2D>("Images/Sprites/ReactionClockIcon");
            reactGameIconImgs[GREEN] = reactGameIconImgs[RED];

            //Load the time bar images
            timeBarImgs[EMPTY] = Content.Load<Texture2D>("Images/Sprites/ProgressBarEmpty");
            timeBarImgs[FULL] = Content.Load<Texture2D>("Images/Sprites/ProgressBarFull");

            //Load the text box image
            textBoxImg = Content.Load<Texture2D>("Images/Sprites/TextBox");

            //Set the end of game icon images
            endgameImgs = pregameImgs;

            //Set the rectangle for the title icon, and the location for the title and the description of the title
            titleIconRec = new Rectangle(screenWidth / 2 - reactionIconImg.Width / 2, TITLE_OFFSET, reactionIconImg.Width, reactionIconImg.Height);
            titleLoc = new Vector2(screenWidth / 2 - titleFont.MeasureString(title).X / 2, titleIconRec.Bottom + TITLE_SPACER);
            titleDescLoc = new Vector2(screenWidth / 2 - instFont.MeasureString(titleDesc).X / 2, titleLoc.Y + titleFont.MeasureString(title).Y + TITLE_SPACER);

            //Set the location for the game names in the menu
            menuReactionLoc = new Vector2(screenWidth / 2 - menuFont.MeasureString(menuSequence).X / 2, (int)(titleDescLoc.Y + instFont.MeasureString(titleDesc).Y + MENU_TEXT_SPACER));
            menuAimLoc = new Vector2(menuReactionLoc.X, menuReactionLoc.Y + menuFont.MeasureString(menuSequence).Y + MENU_TEXT_SPACER);
            menuNumberLoc = new Vector2(menuReactionLoc.X, menuAimLoc.Y + menuFont.MeasureString(menuSequence).Y + MENU_TEXT_SPACER);
            menuSequenceLoc = new Vector2(menuReactionLoc.X, menuNumberLoc.Y + menuFont.MeasureString(menuSequence).Y + MENU_TEXT_SPACER);

            //Set the location for the best scores for each game in the menu
            menuReactionBestLoc = new Vector2(menuReactionLoc.X + menuFont.MeasureString(menuSequence).X + MENU_HORIZ_SPACER, menuReactionLoc.Y);
            menuAimBestLoc = new Vector2(menuReactionBestLoc.X, menuAimLoc.Y);
            menuNumberBestLoc = new Vector2(menuReactionBestLoc.X, menuNumberLoc.Y);
            menuSequenceBestLoc = new Vector2(menuReactionBestLoc.X, menuSequenceLoc.Y);

            //Set the end of game icon rectangles
            endgameIconRecs[REACTION] = new Rectangle(screenWidth / 2 - endgameImgs[REACTION].Width / 2, TITLE_OFFSET, endgameImgs[REACTION].Width, endgameImgs[REACTION].Height);
            endgameIconRecs[AIM] = new Rectangle(screenWidth / 2 - endgameImgs[AIM].Width / 2, TITLE_OFFSET, endgameImgs[AIM].Width, endgameImgs[AIM].Height);
            endgameIconRecs[NUMBER] = new Rectangle(screenWidth / 2 - endgameImgs[NUMBER].Width / 2, TITLE_OFFSET, endgameImgs[NUMBER].Width, endgameImgs[NUMBER].Height);
            endgameIconRecs[SEQUENCE] = new Rectangle(screenWidth / 2 - endgameImgs[SEQUENCE].Width / 2, TITLE_OFFSET, endgameImgs[SEQUENCE].Width, endgameImgs[SEQUENCE].Height);

            //Set the end of game icon rectangles
            endgameIconRecs[REACTION] = new Rectangle(screenWidth / 2 - endgameImgs[REACTION].Width / 2, TITLE_OFFSET, endgameImgs[REACTION].Width, endgameImgs[REACTION].Height);
            endgameIconRecs[AIM] = new Rectangle(screenWidth / 2 - endgameImgs[AIM].Width / 2, TITLE_OFFSET, endgameImgs[AIM].Width, endgameImgs[AIM].Height);
            endgameIconRecs[NUMBER] = new Rectangle(screenWidth / 2 - endgameImgs[NUMBER].Width / 2, TITLE_OFFSET, endgameImgs[NUMBER].Width, endgameImgs[NUMBER].Height);
            endgameIconRecs[SEQUENCE] = new Rectangle(screenWidth / 2 - endgameImgs[SEQUENCE].Width / 2, TITLE_OFFSET, endgameImgs[SEQUENCE].Width, endgameImgs[SEQUENCE].Height);

            //Set the end titles as the pre game titles
            endTitles = preTitle;

            //Set the location for the game titles in the end of game state
            endTitleLocs[REACTION] = new Vector2(screenWidth / 2 - menuFont.MeasureString(endTitles[REACTION]).X / 2, endgameIconRecs[REACTION].Bottom + TITLE_SPACER);
            endTitleLocs[AIM] = new Vector2(screenWidth / 2 - menuFont.MeasureString(endTitles[AIM]).X / 2, endgameIconRecs[AIM].Bottom + TITLE_SPACER);
            endTitleLocs[NUMBER] = new Vector2(screenWidth / 2 - menuFont.MeasureString(endTitles[NUMBER]).X / 2, endgameIconRecs[NUMBER].Bottom + TITLE_SPACER);
            endTitleLocs[SEQUENCE] = new Vector2(screenWidth / 2 - menuFont.MeasureString(endTitles[SEQUENCE]).X / 2, endgameIconRecs[SEQUENCE].Bottom + TITLE_SPACER);

            //Set the location for the saving score messages
            endScoreLoc = new Vector2(0, screenHeight / 2 - titleFont.MeasureString(scoreMsg).Y / 2);
            endSaveScoreLoc = new Vector2(screenWidth / 2 - instFont.MeasureString(endSaveScoreMsg).X / 2, screenHeight - END_VERT_SPACER - END_TEXT_SPACER);

            //Set the rectangles for the game buttons in the menu
            titleBtnRecs[REACTION] = new Rectangle((int)(menuReactionLoc.X - reactionBtns[OFF].Width - MENU_HORIZ_SPACER), (int)(menuReactionLoc.Y - 5), reactionBtns[OFF].Width, reactionBtns[OFF].Height);
            titleBtnRecs[AIM] = new Rectangle(titleBtnRecs[REACTION].X, titleBtnRecs[REACTION].Bottom + MENU_BTN_SPACER, reactionBtns[OFF].Width, reactionBtns[OFF].Height);
            titleBtnRecs[NUMBER] = new Rectangle(titleBtnRecs[REACTION].X, titleBtnRecs[AIM].Bottom + MENU_BTN_SPACER, reactionBtns[OFF].Width, reactionBtns[OFF].Height);
            titleBtnRecs[SEQUENCE] = new Rectangle(titleBtnRecs[REACTION].X, titleBtnRecs[NUMBER].Bottom + MENU_BTN_SPACER, reactionBtns[OFF].Width, reactionBtns[OFF].Height);

            //Set the rectangles for the game icons in the pregame
            preIconRecs[REACTION] = new Rectangle(screenWidth / 2 - pregameImgs[REACTION].Width / 2, screenHeight / 2 - pregameImgs[REACTION].Height / 2, pregameImgs[REACTION].Width, pregameImgs[REACTION].Height);
            preIconRecs[AIM] = new Rectangle(screenWidth / 2 - pregameImgs[AIM].Width / 2, screenHeight / 2 - pregameImgs[AIM].Height / 2, pregameImgs[AIM].Width, pregameImgs[AIM].Height);
            preIconRecs[NUMBER] = new Rectangle(screenWidth / 2 - pregameImgs[NUMBER].Width / 2, screenHeight / 2 - pregameImgs[NUMBER].Height / 2, pregameImgs[NUMBER].Width, pregameImgs[NUMBER].Height);
            preIconRecs[SEQUENCE] = new Rectangle(screenWidth / 2 - pregameImgs[SEQUENCE].Width / 2, screenHeight / 2 - pregameImgs[SEQUENCE].Height / 2, pregameImgs[SEQUENCE].Width, pregameImgs[SEQUENCE].Height);

            //Set the rectangle for the start, save, and try buttons
            startRec = new Rectangle(screenWidth / 2 - startBtns[OFF].Width / 2, screenHeight - PRE_VERT_SPACER, startBtns[OFF].Width, startBtns[OFF].Height);
            saveRec = new Rectangle(screenWidth / 2 - saveBtns[OFF].Width - END_BUTTON_SPACER / 2, screenHeight - END_VERT_SPACER, saveBtns[OFF].Width, saveBtns[OFF].Height);
            tryRec = new Rectangle(screenWidth / 2 + END_BUTTON_SPACER / 2, screenHeight - END_VERT_SPACER, tryBtns[OFF].Width, tryBtns[OFF].Height);

            //Set the location for the pregame title and description
            pregameTitleLoc = new Vector2(0, PRE_VERT_SPACER);
            pregameDescLoc = new Vector2(0, startRec.Y - PRE_TEXT_SPACER);

            //Set the target rectangle and the radius squared
            targetRec = new Rectangle(rng.Next(0, screenWidth - targetImg.Width), rng.Next(0, screenHeight - targetImg.Height), targetImg.Width, targetImg.Height);
            sqrTgtRadius = (int)Math.Pow(targetRec.Width / 2, 2);

            //Set the location for the large reaction display messages
            reactGameMsgLocs[RED] = new Vector2(screenWidth / 2 - titleFont.MeasureString(reactGameMsgs[RED]).X / 2, screenHeight / 2 - titleFont.MeasureString(reactGameMsgs[RED]).Y);
            reactGameMsgLocs[GREEN] = new Vector2(screenWidth / 2 - titleFont.MeasureString(reactGameMsgs[GREEN]).X / 2, screenHeight / 2 - titleFont.MeasureString(reactGameMsgs[GREEN]).Y);
            reactGameMsgLocs[TIME_RECORDED] = new Vector2(screenWidth / 2 - titleFont.MeasureString(reactGameMsgs[TIME_RECORDED]).X / 2, screenHeight / 2);
            reactGameMsgLocs[TOO_EARLY] = new Vector2(screenWidth / 2 - titleFont.MeasureString(reactGameMsgs[TOO_EARLY]).X / 2, screenHeight / 2);

            //Set the reaction game icon rectangles
            reactGameIconRecs[TIME_RECORDED] = new Rectangle(screenWidth / 2 - reactGameIconImgs[TIME_RECORDED].Width / 2,
                                                             screenHeight / 2 - reactGameIconImgs[TIME_RECORDED].Height,
                                                             reactGameIconImgs[TIME_RECORDED].Width,
                                                             reactGameIconImgs[TIME_RECORDED].Height);
            reactGameIconRecs[RED] = new Rectangle(screenWidth / 2 - reactGameIconImgs[RED].Width / 2,
                                                    (int)(reactGameMsgLocs[RED].Y - reactGameIconImgs[RED].Height - TITLE_SPACER),
                                                    reactGameIconImgs[RED].Width,
                                                    reactGameIconImgs[RED].Height);
            reactGameIconRecs[TOO_EARLY] = new Rectangle(screenWidth / 2 - reactGameIconImgs[TOO_EARLY].Width / 2,
                                                         reactGameIconRecs[TIME_RECORDED].Y,
                                                         reactGameIconImgs[TOO_EARLY].Width,
                                                         reactGameIconImgs[TOO_EARLY].Height);
            reactGameIconRecs[GREEN] = reactGameIconRecs[RED];

            //Set the location for the smaller click to continue messages
            reactContinueLocs[RED] = new Vector2(0, 0);
            reactContinueLocs[GREEN] = new Vector2(0, 0);
            reactContinueLocs[TIME_RECORDED] = new Vector2(screenWidth / 2 - instFont.MeasureString(reactContinueMsgs[TIME_RECORDED]).X / 2, reactGameMsgLocs[TIME_RECORDED].Y + titleFont.MeasureString(reactGameMsgs[TIME_RECORDED]).Y + REACT_TEXT_SPACER);
            reactContinueLocs[TOO_EARLY] = new Vector2(screenWidth / 2 - instFont.MeasureString(reactContinueMsgs[TOO_EARLY]).X / 2, reactGameMsgLocs[TOO_EARLY].Y + titleFont.MeasureString(reactGameMsgs[TOO_EARLY]).Y + REACT_TEXT_SPACER);

            //Set the location for the digits in the number game
            digitLoc = new Vector2(screenWidth / 2 - titleFont.MeasureString(digitMsg).X / 2, screenHeight / 2 - titleFont.MeasureString(digitMsg).Y / 2);

            //Set the rectangles for the time bar
            timeBarRecs[EMPTY] = new Rectangle(screenWidth / 2 - timeBarImgs[EMPTY].Width / 2, (int)(digitLoc.Y + titleFont.MeasureString(digitMsg).Y + TITLE_OFFSET), timeBarImgs[EMPTY].Width, timeBarImgs[EMPTY].Height);
            timeBarRecs[FULL] = new Rectangle(screenWidth / 2 - timeBarImgs[FULL].Width / 2, (int)(digitLoc.Y + titleFont.MeasureString(digitMsg).Y + TITLE_OFFSET), timeBarImgs[FULL].Width, timeBarImgs[FULL].Height);

            //Set the rectangle for the text box
            textBoxRec = new Rectangle(0, screenHeight / 2 - textBoxImg.Height / 2, screenWidth, textBoxImg.Height);

            //Set the location for the instructional prompts in the number game
            enterInstrLoc = new Vector2(screenWidth / 2 - instFont.MeasureString(enterInstrMsg).X / 2, textBoxRec.Y - TITLE_SPACER - menuFont.MeasureString(enterInstrMsg).Y);
            digitQuestionLoc = new Vector2(screenWidth / 2 - menuFont.MeasureString(digitQuestion).X / 2, enterInstrLoc.Y - TITLE_SPACER - menuFont.MeasureString(digitQuestion).Y);

            //Set the location for the user's answer
            userAnswerLoc = new Vector2(screenWidth / 2, screenHeight / 2 - numberFont.MeasureString(userAnswer).Y / 2);

            //Set the locations for the number label, correct number, user answer label
            numLblLoc = new Vector2(screenWidth / 2 - instFont.MeasureString(numberLabel).X / 2, TITLE_SPACER);
            corNumLoc = new Vector2(screenWidth / 2, numLblLoc.Y + instFont.MeasureString(numberLabel).Y + TITLE_SPACER);
            userAnsLblLoc = new Vector2(screenWidth / 2 - instFont.MeasureString(userAnswerLabel).X / 2, corNumLoc.Y + numberFont.MeasureString(digitMsg).Y + TITLE_SPACER);
            
            //Set the location and color for the user's digits in the end game state of the number game
            for (int i = 0; i < userDgtLocs.Length; i++)
            {
                //Set each location and color for each of the user's digits in the end game state
                userDgtLocs[i] = new Vector2(screenWidth / 2, userAnsLblLoc.Y + instFont.MeasureString(userAnswerLabel).Y + TITLE_SPACER);
                userDgtColors[i] = right;
            }
            
            //Set the target timer
            targetTimer = new Timer(Timer.INFINITE_TIMER, false);
            
            //Set the reaction timer
            reactionTimer = new Timer(Timer.INFINITE_TIMER, false);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //Get the current and previous mouse state
            prevMouse = mouse;
            mouse = Mouse.GetState();

            //Update the game according to the current game state
            switch(gameState)
            {
                case MENU:
                    //Update the menu
                    UpdateMenu();
                    break;

                case PREGAME:
                    //Update the pregame state
                    UpdatePreGame();
                    break;

                case GAME:
                    //Update the game state
                    UpdateGame(gameTime);
                    break;

                case ENDGAME:
                    //Update the end game state
                    UpdateEndGame();
                    break;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //Set the background color
            GraphicsDevice.Clear(curBgColor);

            spriteBatch.Begin();

            //Draw the game according to the current game state
            switch (gameState)
            {
                //Draw the game screen depending on the state of the game
                case MENU:
                    //Draw the menu screen
                    DrawMenu();
                    break;

                case PREGAME:
                    //Draw the pregame screen
                    DrawPreGame();
                    break;

                case GAME:
                    //Draw the game screen
                    DrawGame();
                    break;

                case ENDGAME:
                    //Draw the end of game screen
                    DrawEndGame();
                    break;
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        //Pre: None
        //Post: None
        //Desc: Update the menu by checking for a mouse hover or click over any game buttons and switching to the chosen game
        private void UpdateMenu()
        {
            //Set whether the mouse is hovering over the game buttons in the menu
            hoverReactionBtn = titleBtnRecs[REACTION].Contains(mouse.Position);
            hoverAimBtn = titleBtnRecs[AIM].Contains(mouse.Position);
            hoverNumberBtn = titleBtnRecs[NUMBER].Contains(mouse.Position);
            hoverSequenceBtn = titleBtnRecs[SEQUENCE].Contains(mouse.Position);

            //Check for the mouse click
            if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
            {
                //Set up the pregame screen according to the game that was clicked on
                if (titleBtnRecs[REACTION].Contains(mouse.Position))
                {
                    //Set up the pregame for the reaction game
                    SetupPreGame(REACTION);
                }
                else if (titleBtnRecs[AIM].Contains(mouse.Position))
                {
                    //Set up the pregame for the aim game
                    SetupPreGame(AIM);
                }
                else if (titleBtnRecs[NUMBER].Contains(mouse.Position))
                {
                    //Set up the pregame for the number game
                    SetupPreGame(NUMBER);
                }
                else if (titleBtnRecs[SEQUENCE].Contains(mouse.Position))
                {
                    //Set up the pregame for the sequence game
                    SetupPreGame(SEQUENCE);
                }
            }
        }

        //Pre: None
        //Post: None
        //Desc: Update the pre game state by checking for a mouse click on the start button and setting up the game
        private void UpdatePreGame()
        {
            //Set whether the mouse is hovering over the start button
            startHoverBtn = startRec.Contains(mouse.Position);

            //Reset the game if the mouse clicks on the screen
            if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
            {
                //Reset the game based on the chosen game type
                ResetGame();                
            }
        }

        //Pre: The parameter gameTime has a positive value of the time passed between updates for the elapsed game time
        //Post: None
        //Desc: Update the game depending on the current game type that the user is playing
        private void UpdateGame(GameTime gameTime)
        {
            //Update the game depending on the current game type
            switch (gameType)
            {
                case REACTION:
                    //Update the reaction time game
                    UpdateReaction(gameTime);
                    break;

                case AIM:
                    //Update the aim trainer game
                    UpdateAim(gameTime);
                    break;

                case NUMBER:
                    //Update the number memory game
                    UpdateNumber(gameTime);
                    break;

                case SEQUENCE:
                    //Update the sequence memory game
                    UpdateSequence(gameTime);
                    break;
            }
        }

        //Pre: The parameter gameTime has a positive value of the time passed between updates for the elapsed game time\
        //Post: None
        //Desc: Update the reaction game based on whether the state of the game is red, green, too early, or the time is recorded
        private void UpdateReaction(GameTime gameTime)
        {        
            //Update the reaction game depending on which state the reaction time game is at
            switch (reactionState)
            {
                case RED:
                    //Update the wait timer
                    waitTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

                    //Change the state to green or to too early depending if the wait timer finished or if the mouse clicked too early
                    if (waitTimer.IsFinished())
                    {
                        //Change the background color to green and the state to green
                        curBgColor = clickColor;
                        reactionState = GREEN;

                        //Start the reaction timer
                        reactionTimer.ResetTimer(true);
                    }
                    else if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
                    {
                        //Set the background color as the regular background color and change to the oops state
                        curBgColor = regularBgColor;
                        reactionState = TOO_EARLY;
                    }
                    break;

                case GREEN:
                    //Update the reaction timer
                    reactionTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

                    //Record the reaction time when the mouse clicks
                    if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
                    {
                        //Set the background color as the regular background color
                        curBgColor = regularBgColor;

                        //Record the time that the mouse clicked as the reaction time
                        reactionTimes[reactionRound - 1] = reactionTimer.GetTimePassed();

                        //Change the state to the time recorded screen or the end game screen depending on the round number
                        if (reactionRound >= REACT_MAX_ROUNDS)
                        {
                            //Change the game state to the end game state
                            gameState = ENDGAME;

                            //Calculate the average reaction time
                            score = (int)Math.Round(reactionTimes.Sum() / REACT_MAX_ROUNDS, 0);

                            //Set the score message and center it on the screen
                            scoreMsg = score + " ms";
                            endScoreLoc.X = screenWidth / 2 - titleFont.MeasureString(scoreMsg).X / 2;
                        }
                        else
                        {
                            //Change the reaction state to the time recorded state
                            reactionState = TIME_RECORDED;

                            //Set the score message and center it on the screen
                            reactGameMsgs[TIME_RECORDED] = Math.Round(reactionTimes[reactionRound - 1], 0) + " ms";
                            reactGameMsgLocs[TIME_RECORDED].X = screenWidth / 2 - titleFont.MeasureString(reactGameMsgs[TIME_RECORDED]).X / 2;
                        }
                    }
                    break;

                case TOO_EARLY:
                    //Change back to the waiting screen when the mouse clicks again
                    if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
                    {
                        //Reset the reaction round
                        ResetReactionRound();
                    }
                    break;

                case TIME_RECORDED:
                    //Start a new round once the mouse is clicked
                    if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
                    {
                        //Increase the reaction round number
                        reactionRound++;

                        //Reset the reaction round
                        ResetReactionRound();
                    }
                    break;
            }   
        }

        //Pre: The parameter gameTime has a positive value of the time passed between updates for the elapsed game time
        //Post: None
        //Desc: Update the aim trainer game by checking for target clicks, recording the reaction time, and generating new targets
        private void UpdateAim(GameTime gameTime)
        {
            //Update the target timer
            targetTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

            //Check whether the mouse clicked
            if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
            {
                //Calculate the squared distance from the target center
                sqrDistFromTgtCntr = Math.Pow(mouse.Position.X - targetRec.Center.X, 2) + Math.Pow(mouse.Position.Y - targetRec.Center.Y, 2);

                //Check whether the mouse is inside the circle to decrease the targets remaining
                if (sqrDistFromTgtCntr <= sqrTgtRadius)
                {
                    //Increase the number of targets clicked by 1 decrease targets remaining by 1
                    targetsRemaining--;

                    //Update the targets remaining message and location
                    aimTargetsRemainingMsg = "Remaining " + targetsRemaining;
                    aimTgtRemainingLoc.X = screenWidth / 2 - menuFont.MeasureString(aimTargetsRemainingMsg).X / 2;

                    //Store the aim time in the next element of the aim times array
                    aimTimes[targetsRemaining] = targetTimer.GetTimePassed();

                    //Generate a new target
                    GenerateNewTarget();
                }
            }
            
            //Check whether the game is over if all 30 targets have been clicked to end the game
            if (targetsRemaining <= 0)
            {
                //Set the average time score for aim and the score message
                score = (int)Math.Round(aimTimes.Sum() / aimTimes.Length, 0);
                scoreMsg = score + " ms";

                //Recenter the score message 
                endScoreLoc.X = screenWidth / 2 - titleFont.MeasureString(scoreMsg).X / 2;

                //Change the game state to end game 
                gameState = ENDGAME;
            }
        }

        //Pre: The parameter gameTime has a positive value of the time passed between updates for the elapsed game time
        //Post: None
        //Desc: Update the number memory game depending if the state is memorizing or the typing state 
        private void UpdateNumber(GameTime gameTime)
        {
            //Get the current and previous keyboard state
            prevKb = kb;
            kb = Keyboard.GetState();
            
            //Update the number game depending on the state that the game is in
            switch (numberState)
            {
                case MEMORIZING:
                    //Update the memorizing timer
                    memorizingTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

                    //Update the time bar percentage and the width of the full time bar
                    barPercentage = (float)memorizingTimer.GetTimeRemaining() / memorizingTime;
                    timeBarRecs[FULL].Width = (int)(barPercentage * timeBarImgs[FULL].Width);

                    //Change the state to typing when the memorizing timer is finished
                    if (memorizingTimer.IsFinished())
                    {
                        //Set the number state as typing
                        numberState = TYPING;

                        //Clear the user's answer
                        userAnswer = "";
                    }
                    break;

                case TYPING:
                    //Allow for a digit to be added if the user's answer is shorter than the round number
                    if (userAnswer.Length < numberRound)
                    {
                        //Add a 0 to the user's answer if it was not pressed in the previous update
                        if (kb.IsKeyDown(Keys.D0) && prevKb.IsKeyUp(Keys.D0))
                        {
                            //Add a 0 to the user's answer
                            userAnswer += "0";
                        }
                        else if (kb.IsKeyDown(Keys.D1) && prevKb.IsKeyUp(Keys.D1))
                        {
                            //Add a 1 to the user's answer
                            userAnswer += "1";
                        }
                        else if (kb.IsKeyDown(Keys.D2) && prevKb.IsKeyUp(Keys.D2))
                        {
                            //Add a 2 to the user's answer
                            userAnswer += "2";
                        }
                        else if (kb.IsKeyDown(Keys.D3) && prevKb.IsKeyUp(Keys.D3))
                        {
                            //Add a 3 to the user's answer
                            userAnswer += "3";
                        }
                        else if (kb.IsKeyDown(Keys.D4) && prevKb.IsKeyUp(Keys.D4))
                        {
                            //Add a 4 to the user's answer
                            userAnswer += "4";
                        }
                        else if (kb.IsKeyDown(Keys.D5) && prevKb.IsKeyUp(Keys.D5))
                        {
                            //Add a 5 to the user's answer
                            userAnswer += "5";
                        }
                        else if (kb.IsKeyDown(Keys.D6) && prevKb.IsKeyUp(Keys.D6))
                        {
                            //Add a 6 to the user's answer
                            userAnswer += "6";
                        }
                        else if (kb.IsKeyDown(Keys.D7) && prevKb.IsKeyUp(Keys.D7))
                        {
                            //Add a 7 to the user's answer
                            userAnswer += "7";
                        }
                        else if (kb.IsKeyDown(Keys.D8) && prevKb.IsKeyUp(Keys.D8))
                        {
                            //Add a 8 to the user's answer
                            userAnswer += "8";
                        }
                        else if (kb.IsKeyDown(Keys.D9) && prevKb.IsKeyUp(Keys.D9))
                        {
                            //Add a 9 to the user's answer
                            userAnswer += "9";
                        }

                        //Recenter the user's answer in the text box
                        userAnswerLoc.X = screenWidth / 2 - numberFont.MeasureString(userAnswer).X / 2;
                    }
                    
                    //Remove the last digit if the user's answer has a digit in it and backspace has not been pressed in the previous update
                    if (kb.IsKeyDown(Keys.Back) && prevKb.IsKeyUp(Keys.Back) && userAnswer.Length > 0)
                    {
                        //Remove the last digit from the user's answer
                        userAnswer = userAnswer.Remove(userAnswer.Length - 1);

                        //Recenter the user's answer in the text box
                        userAnswerLoc.X = screenWidth / 2 - numberFont.MeasureString(userAnswer).X / 2;
                    }

                    //Check whether the answer is correct when the user entered the right amount of digits
                    if (kb.IsKeyDown(Keys.Enter) && prevKb.IsKeyUp(Keys.Enter) && digitMsg.Length == userAnswer.Length)
                    {
                        //Set whether each of the user's digits matches the correct answer by marking the digit's color
                        for (int j = 0; j < numberRound; j++)
                        {
                            //Set the digit's color as black for wrong if the digit doesn't match the answer
                            if (numberDigits[j] != (userAnswer[j] - '0'))
                            {
                                //Set the color of the digit as the color for wrong
                                userDgtColors[j] = wrong;
                            }
                        }

                        //Change to the end of game state if any digits are marked as wrong or if the level is the max level
                        if (userDgtColors.Contains(wrong) || numberRound == NUMER_MAX_LEVEL)
                        {
                            //Change the game state to the end of game
                            gameState = ENDGAME;

                            //Increase the round number if the last level was correct
                            if (!userDgtColors.Contains(wrong) && numberRound == NUMER_MAX_LEVEL)
                            {
                                //Increase the round number or the points by 1
                                numberRound++;
                            }
                         
                            //Set the score message and its location
                            scoreMsg = "Level " + numberRound;
                            endScoreLoc.Y = screenHeight / 2;
                            endScoreLoc.X = screenWidth / 2 - titleFont.MeasureString(scoreMsg).X / 2;

                            //Set the location for the correct number
                            corNumLoc.X = screenWidth / 2 - numberFont.MeasureString(digitMsg).X / 2;

                            //Set the location for the first digit
                            userDgtLocs[0].X = screenWidth / 2 - numberFont.MeasureString(userAnswer).X / 2;
                            
                            //Set the location and color for each of the user's digits after the first digit
                            for (int j = 1; j < userAnswer.Length; j++)
                            {
                                //Set the location of each digit after the first digit
                                userDgtLocs[j].X = userDgtLocs[j - 1].X + numberFont.MeasureString(userAnswer[j - 1].ToString()).X;                              
                            }                         
                        }
                        else
                        {
                            //Increase the round number
                            numberRound++;
                            
                            //Change the number state to memorizing
                            numberState = MEMORIZING;

                            //Generate a new number
                            GenerateNewDigits();

                            //Increase the memorizing and reset the memorizing timer
                            memorizingTime += MEMORIZING_TIME_INCREMENT;
                            memorizingTimer = new Timer(memorizingTime, true);
                        }
                    }

                    break;   
            }
        }

        //Pre: The parameter gameTime has a positive value of the time passed between updates for the elapsed game time
        //Post: None
        //Desc: Update the sequence memory game
        private void UpdateSequence(GameTime gameTime)
        {

        }

        //Pre: None
        //Post: None
        //Desc: Update the end of game state by checking for a mouse click on the save or try again button
        private void UpdateEndGame()
        {
            //Set whether the mouse is hovering over the save or try again buttons
            saveHoverBtn = saveRec.Contains(mouse.Position);
            tryHoverBtn = tryRec.Contains(mouse.Position);

            //Set the game state if the mouse is pressed on the save or try again button
            if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
            {
                //Change the game state to menu or game according to the button that the mouse is hovering over
                if (saveHoverBtn)
                {
                    //Change the game state to menu
                    gameState = MENU;

                    //Update the menu best scores depending on which game was played
                    switch (gameType)
                    {
                        case REACTION:
                            //Check whether the current reaction score is better than its high score
                            if (score < reactionBestScore || reactionBestScore == -1)
                            {
                                //Set the high score of the reaction game and update the high score in the menu
                                reactionBestScore = score;
                                menuReactionBest = reactionBestScore + " ms";
                            }
                            break;

                        case AIM:
                            //Check whether the current aim score is better than the high score
                            if (score < aimBestScore || aimBestScore == -1)
                            {
                                //Set the high score of the aim game and update the high score in the menu
                                aimBestScore = score;
                                menuAimBest = aimBestScore + " ms";
                            }
                            break;

                        case NUMBER:
                            //Check whether the current number score is better than the high score
                            if (numberRound > numberBestScore)
                            {
                                //Set the high score of the number game and update the high score in the menu
                                numberBestScore = numberRound;
                                menuNumberBest = numberBestScore + " pts";
                            }
                            break;

                        case SEQUENCE:
                            break;
                    }                    
                }
                else if (tryHoverBtn)
                {
                    //Change the game state to game and reset the game
                    gameState = GAME;
                    ResetGame();
                }
            }
        }

        //Pre: None
        //Post: None
        //Desc: Draw the menu screen with the icons, buttons, and text
        private void DrawMenu()
        {
            //Draw the game icon, title, and description
            spriteBatch.Draw(reactionIconImg, titleIconRec, Color.White);
            spriteBatch.DrawString(titleFont, title, titleLoc, Color.White);
            spriteBatch.DrawString(instFont, titleDesc, titleDescLoc, Color.White);

            //Draw the various game buttons
            spriteBatch.Draw(reactionBtns[Convert.ToInt32(hoverReactionBtn)], titleBtnRecs[REACTION], Color.White);
            spriteBatch.Draw(aimBtns[Convert.ToInt32(hoverAimBtn)], titleBtnRecs[AIM], Color.White);
            spriteBatch.Draw(numberBtns[Convert.ToInt32(hoverNumberBtn)], titleBtnRecs[NUMBER], Color.White);
            spriteBatch.Draw(sequenceBtns[Convert.ToInt32(hoverSequenceBtn)], titleBtnRecs[SEQUENCE], Color.White);
            
            //Draw the mini game titles
            spriteBatch.DrawString(menuFont, menuReaction, menuReactionLoc, Color.White);
            spriteBatch.DrawString(menuFont, menuAim, menuAimLoc, Color.White);
            spriteBatch.DrawString(menuFont, menuNumber, menuNumberLoc, Color.White);
            spriteBatch.DrawString(menuFont, menuSequence, menuSequenceLoc, Color.White);

            //Draw the mini games best scores
            spriteBatch.DrawString(menuFont, menuReactionBest, menuReactionBestLoc, Color.Black);
            spriteBatch.DrawString(menuFont, menuAimBest, menuAimBestLoc, Color.Black);
            spriteBatch.DrawString(menuFont, menuNumberBest, menuNumberBestLoc, Color.Black);
            spriteBatch.DrawString(menuFont, menuSequenceBest, menuSequenceBestLoc, Color.Black);
        }

        //Pre: None
        //Post: None
        //Desc: Draw the pregame screen with the icons, buttons, and text
        private void DrawPreGame()
        {
            //Draw the game icon, title, instructions, and the start button
            spriteBatch.Draw(pregameImgs[gameType], preIconRecs[gameType], Color.White);
            spriteBatch.DrawString(titleFont, preTitle[gameType], pregameTitleLoc, Color.White);
            spriteBatch.DrawString(instFont, preDesc[gameType], pregameDescLoc, Color.White);
            spriteBatch.Draw(startBtns[Convert.ToInt32(startHoverBtn)], startRec, Color.White);
        }

        //Pre: None
        //Post: None
        //Desc: Draw the game screen depending on the chosen game
        private void DrawGame()
        {
            //Draw the game depending on the game type
            switch (gameType)
            {
                case REACTION:
                    //Draw the reaction game
                    DrawReaction();
                    break;

                case AIM:
                    //Draw the aim game
                    DrawAim();
                    break;

                case NUMBER:
                    //Draw the number game
                    DrawNumber();
                    break;

                case SEQUENCE:
                    //Draw the sequence game
                    DrawSequence();
                    break;
            }
        }

        //Pre: None
        //Post: None
        //Desc: Draw the end game screen with the corresponding game's imagery, buttons, and text
        private void DrawEndGame()
        {
            //Draw the top part of end game screen with the game icon and title for all games other than the number game
            if (gameType != NUMBER)
            {
                //Draw the game icon
                spriteBatch.Draw(endgameImgs[gameType], endgameIconRecs[gameType], Color.White);

                //Draw the game title
                spriteBatch.DrawString(menuFont, endTitles[gameType], endTitleLocs[gameType], Color.White);
            }
            else
            {
                //Draw the number label, correct number, user answer label
                spriteBatch.DrawString(instFont, numberLabel, numLblLoc, Color.White);
                spriteBatch.DrawString(numberFont, digitMsg, corNumLoc, Color.White);
                spriteBatch.DrawString(instFont, userAnswerLabel, userAnsLblLoc, Color.White);

                //Draw each of the user's digits with their color
                for (int i = 0; i < userAnswer.Length; i++)
                {
                    //Draw the digit with its specific color
                    spriteBatch.DrawString(numberFont, userAnswer[i].ToString(), userDgtLocs[i], userDgtColors[i]);
                }
            }

            //Draw the score of the game
            spriteBatch.DrawString(titleFont, scoreMsg, endScoreLoc, Color.White);

            //Draw the message that tells the user to save the score with the button
            spriteBatch.DrawString(instFont, endSaveScoreMsg, endSaveScoreLoc, Color.White);

            //Draw the save and try again buttons
            spriteBatch.Draw(saveBtns[Convert.ToInt32(saveHoverBtn)], saveRec, Color.White);
            spriteBatch.Draw(tryBtns[Convert.ToInt32(tryHoverBtn)], tryRec, Color.White);
        }

        //Pre: None
        //Post: None
        //Desc: Draw the reaction time game's text in the game play
        private void DrawReaction()
        {
            //Draw the reaction game icon and the reaction game messages
            spriteBatch.Draw(reactGameIconImgs[reactionState], reactGameIconRecs[reactionState], Color.White);
            spriteBatch.DrawString(titleFont, reactGameMsgs[reactionState], reactGameMsgLocs[reactionState], Color.White);
            spriteBatch.DrawString(instFont, reactContinueMsgs[reactionState], reactContinueLocs[reactionState], Color.White);
        }

        //Pre: None
        //Post: None
        //Desc: Draw the aim trainer game's target and text in the game play
        private void DrawAim()
        {
            //Draw the remaining target message and the target
            spriteBatch.DrawString(menuFont, aimTargetsRemainingMsg, aimTgtRemainingLoc, Color.White);
            spriteBatch.Draw(targetImg, targetRec, Color.White);
        }

        //Pre: None
        //Post: None
        //Desc: Draw the number memory game's images and text in the game play
        private void DrawNumber()
        {
            //Draw the memorizing or typing state depending on the number game state
            switch (numberState)
            { 
                case MEMORIZING:
                    //Draw the digits
                    spriteBatch.DrawString(titleFont, digitMsg, digitLoc, Color.White);

                    //Draw the progress bar
                    spriteBatch.Draw(timeBarImgs[EMPTY], timeBarRecs[EMPTY], Color.White);
                    spriteBatch.Draw(timeBarImgs[FULL], timeBarRecs[FULL], Color.White);
                    break;

                case TYPING:
                    //Draw the instructional messages
                    spriteBatch.DrawString(menuFont, digitQuestion, digitQuestionLoc, Color.White);
                    spriteBatch.DrawString(instFont, enterInstrMsg, enterInstrLoc, Color.White);

                    //Draw the text box
                    spriteBatch.Draw(textBoxImg, textBoxRec, Color.White);

                    //Draw the user's answer
                    spriteBatch.DrawString(numberFont, userAnswer, userAnswerLoc, Color.White);
                    break;
            }            
        }

        //Pre: None
        //Post: None
        //Desc: Draw the sequence memory gameplay
        private void DrawSequence()
        {

        }

        //Pre: None
        //Post: None
        //Desc: Setup the pregame state by changing the game type and state and centering the game title
        private void SetupPreGame(int newType)
        {
            //Set the game type, change the game state to pregame and update the location for the title and description for the game
            gameType = newType;
            gameState = PREGAME;
            pregameTitleLoc.X = screenWidth / 2 - titleFont.MeasureString(preTitle[gameType]).X / 2;
            pregameDescLoc.X = screenWidth / 2 - instFont.MeasureString(preDesc[gameType]).X / 2;
        }

        //Pre: None
        //Post: None
        //Desc: Generate a randomized new target for the aim trainer
        private void GenerateNewTarget()
        {
            //Randomize the mext target location
            targetRec.X = rng.Next(0, screenWidth - targetImg.Width + 1);
            targetRec.Y = rng.Next((int)menuFont.MeasureString(aimTargetsRemainingMsg).Y, screenHeight - targetImg.Height + 1);

            //Reset the target timer
            targetTimer.ResetTimer(true);
        }

        //Pre: None
        //Post: None
        //Desc: Generate a new number for the number memory
        private void GenerateNewDigits()
        {
            //Clear the digits message
            digitMsg = "";

            //Setup the digit values in the array of digits
            for (int i = 0; i < numberRound; i++)
            {
                //Randomize the digit value in each element
                numberDigits[i] = rng.Next(0, 9);

                //Add the digit to the digit message
                digitMsg += numberDigits[i];
            }

            //Update the location of the digit message
            digitLoc.X = screenWidth / 2 - titleFont.MeasureString(digitMsg).X / 2;
        }

        //Pre: None
        //Post: None
        //Desc: Reset the reaction time round by setting the background, state, and generating a new timer
        private void ResetReactionRound()
        {
            //Set the state as wait
            reactionState = RED;

            //Set the background color as the waiting color
            curBgColor = waitColor;

            //Randomize a new wait time and create a new wait timer
            waitTime = rng.Next(MIN_WAIT_TIME, MAX_WAIT_TIME + 1);
            waitTimer = new Timer(waitTime, true);
        }

        //Pre: None
        //Post: None
        //Desc: Reset the game depending on the game that will be played
        private void ResetGame()
        {
            //Reset the game depending on the current type of game
            switch (gameType)
            {
                case REACTION:
                    //Change the game state to game
                    gameState = GAME;

                    //TODO: Reaction Game Setup
                    //Reset the reaction round and set the round number as 1
                    ResetReactionRound();
                    reactionRound = 1;
                    break;

                case AIM:
                    //Change the game state to game
                    gameState = GAME;

                    //TODO: Aim Trainer Game Setup
                    //Set the targets remaining as the max number of targets
                    targetsRemaining = MAX_TARGETS;

                    //Set the remaining target message and location
                    aimTargetsRemainingMsg = "Remaining " + targetsRemaining;
                    aimTgtRemainingLoc.X = screenWidth / 2 - menuFont.MeasureString(aimTargetsRemainingMsg).X / 2;

                    //Generate the first target on the screen
                    GenerateNewTarget();
                    break;

                case NUMBER:
                    //Change the game state to game
                    gameState = GAME;

                    //TODO: Number Memory Game Setup
                    //Set the number round as 1 and the number state as memorizing
                    numberRound = 1;
                    numberState = MEMORIZING;

                    //Reset all the digit colors to the color that represents right
                    for (int i = 0; i < userDgtColors.Length; i++)
                    {
                        //Reset the color for the digits that are not matching
                        if (userDgtColors[i] == wrong)
                        {
                            //Set every element as the right color 
                            userDgtColors[i] = right;
                        }                        
                    }

                    //Set the memorizing timer and the memorizing time
                    memorizingTime = MIN_MEMORIZING_TIME;
                    memorizingTimer = new Timer(memorizingTime, true);
                       
                    //Generate the digits in the number round
                    GenerateNewDigits();
                    break;

                case SEQUENCE:
                    //Change the game state to game
                    gameState = GAME;

                    //TODO: Sequence Memory Game Setup
                    break;
            }
        }
    }
}

