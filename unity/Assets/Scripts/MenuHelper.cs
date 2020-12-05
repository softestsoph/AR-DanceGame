﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace PoseTeacher
{
    public enum Menus
    {
        TITLE, DANCE, COURSES, SPECCOURSE, SETTINGS, AVATARSETTINGS
    }
    public enum MenuState
    {
        INMAIN, PAUSED
    }

    public class MenuHelper : MonoBehaviour
    {

        public GameObject MenuObject;
        private Dictionary<Menus, GameObject> menus = new Dictionary<Menus, GameObject>();
        private Menus CurrentMenu;
        private Menus PreviousMainMenu;
        private CourseMenuHelper courseMenuHelper;
        private TextMeshPro TitleText;
        public MenuState state = MenuState.INMAIN;

        public GameObject TrainingElements;
        public GameObject CoreoElements;
        private bool isTrainingPause = true;

        public void setState(string state)
        {
            switch (state)
            {
                case "PAUSED_training":
                    this.state = MenuState.PAUSED;
                    // Set the menu to settings, but save state
                    PreviousMainMenu = CurrentMenu;
                    CurrentMenu = Menus.SETTINGS;
                    UpdateMenu();
                    // Show menu
                    MenuObject.SetActive(true);
                    // Change Buttons
                    MenuObject.transform.Find("TitleBarHolder").Find("Buttons").Find("MainButtons").gameObject.SetActive(false);
                    MenuObject.transform.Find("TitleBarHolder").Find("Buttons").Find("PausedButtons").gameObject.SetActive(true);
                    TrainingElements.SetActive(false);
                    isTrainingPause = true;
                    break;
                case "PAUSED_coreo":
                    this.state = MenuState.PAUSED;
                    // Set the menu to settings, but save state
                    PreviousMainMenu = CurrentMenu;
                    CurrentMenu = Menus.SETTINGS;
                    UpdateMenu();
                    // Show menu
                    MenuObject.SetActive(true);
                    // Change Buttons
                    MenuObject.transform.Find("TitleBarHolder").Find("Buttons").Find("MainButtons").gameObject.SetActive(false);
                    MenuObject.transform.Find("TitleBarHolder").Find("Buttons").Find("PausedButtons").gameObject.SetActive(true);
                    CoreoElements.SetActive(false);
                    isTrainingPause = false;
                    break;
                case "INMAIN":
                    this.state = MenuState.INMAIN;
                    // Revert Menu state
                    CurrentMenu = PreviousMainMenu;
                    UpdateMenu();
                    // Hide Menu
                    MenuObject.SetActive(false);
                    // change buttins
                    MenuObject.transform.Find("TitleBarHolder").Find("Buttons").Find("MainButtons").gameObject.SetActive(true);
                    MenuObject.transform.Find("TitleBarHolder").Find("Buttons").Find("PausedButtons").gameObject.SetActive(false);
                    if (isTrainingPause)
                        TrainingElements.SetActive(true);
                    else
                        CoreoElements.SetActive(true);
                    break;
            }
        }

        private void Start()
        {
            Transform MenuHolderTr = MenuObject.transform.Find("NearMenu");
            menus.Add(Menus.TITLE, MenuHolderTr.Find("TitleMenu").gameObject);
            menus.Add(Menus.DANCE, MenuHolderTr.Find("DanceMenu").gameObject);
            menus.Add(Menus.COURSES, MenuHolderTr.Find("CourseMenu").gameObject);
            menus.Add(Menus.SPECCOURSE, MenuHolderTr.Find("SpecCourseMenu").gameObject);
            menus.Add(Menus.SETTINGS, MenuHolderTr.Find("SettingsMenu").gameObject);
            menus.Add(Menus.AVATARSETTINGS, MenuObject.transform.Find("AvatarSettings").gameObject);

            CurrentMenu = Menus.TITLE;

            courseMenuHelper = MenuObject.transform.Find("CourseMenuHelper").gameObject.GetComponent<CourseMenuHelper>();
            courseMenuHelper.LoadAllCourseInfos();
            courseMenuHelper.GenerateCoursesMenu(menus[Menus.COURSES].transform.Find("CourseMenuButtonCollection").gameObject);
            // TODO: create the buttons from code

            TitleText = MenuObject.transform.Find("TitleBarHolder").Find("TitleBar").Find("Title").gameObject.GetComponent<TextMeshPro>();
            SetTitleBarText();
        }

        public void BackToHome()
        {
            if (CurrentMenu != Menus.TITLE)
            {
                // Set current Menu to Inactive
                menus[CurrentMenu].SetActive(false);
                // Set Title Menu to Active
                CurrentMenu = Menus.TITLE;
                menus[CurrentMenu].SetActive(true); 
            }

            SetTitleBarText();
        }

        public void BackToPreviousMenu()
        {
            // Set current Menu to Inactive
            if (CurrentMenu != Menus.TITLE)
                menus[CurrentMenu].SetActive(false);

            // Set Upper level Menu to Active
            switch (CurrentMenu)
            {
                case Menus.TITLE:
                    break;
                case Menus.DANCE:
                    CurrentMenu = Menus.TITLE;
                    menus[CurrentMenu].SetActive(true);
                    break;
                case Menus.COURSES:
                    CurrentMenu = Menus.DANCE;
                    menus[CurrentMenu].SetActive(true);
                    break;
                case Menus.SPECCOURSE:
                    CurrentMenu = Menus.COURSES;
                    menus[CurrentMenu].SetActive(true);
                    break;
                case Menus.SETTINGS:
                    CurrentMenu = Menus.TITLE;
                    menus[CurrentMenu].SetActive(true);
                    break;
                case Menus.AVATARSETTINGS:
                    CurrentMenu = Menus.SETTINGS;
                    menus[CurrentMenu].SetActive(true);
                    break;
                default:
                    break;
            }

            SetTitleBarText();
        }

        public void BackToGame()
        {
            setState("INMAIN");
        }

        public void BackToSetting()
        {
            menus[CurrentMenu].SetActive(false);
            CurrentMenu = Menus.SETTINGS;
            menus[CurrentMenu].SetActive(true);
        }

        private void UpdateMenu()
        {
            foreach (KeyValuePair<Menus, GameObject> menu in menus)
            {
                menu.Value.SetActive(false);
            }
            menus[CurrentMenu].SetActive(true);
            SetTitleBarText();
        }

        public void SelectedMenuOption(int selectedMenuOption)
        {

            switch (CurrentMenu)
            {
                case Menus.TITLE:
                    SelectedInTitleMenu(selectedMenuOption);
                    break;
                case Menus.DANCE:
                    SelectedInDanceMenu(selectedMenuOption);
                    break;
                case Menus.COURSES:
                    SelectedInCoursesMenu(selectedMenuOption);
                    break;
                case Menus.SPECCOURSE:
                    // This is ccurently done in CourseMenuHelper StartStep/StartCoreography function
                    break;
                case Menus.SETTINGS:
                    SelectedInSettingsMenu(selectedMenuOption);
                    break;
                default:
                    break;
            }

            SetTitleBarText();
        }

        private void SelectedInTitleMenu(int selectedMenuOption)
        {
            switch (selectedMenuOption)
            {
                // Title -> Dance Menu
                case 0:
                    menus[CurrentMenu].SetActive(false);
                    CurrentMenu = Menus.DANCE;
                    menus[CurrentMenu].SetActive(true);
                    break;
                // Title -> Workout Menu
                case 1:
                    /*   menus[CurrentMenu].SetActive(false);
                       CurrentMenu = Menus.WORKOUT;
                       menus[CurrentMenu].SetActive(true); */
                    break;
                // Title -> Settings Menu
                case 2:
                    menus[CurrentMenu].SetActive(false);
                    CurrentMenu = Menus.SETTINGS;
                    menus[CurrentMenu].SetActive(true);
                    break;
                default:
                    break;
            }
        }

        private void SelectedInDanceMenu(int selectedMenuOption)
        {
            switch (selectedMenuOption)
            {
                // Dance -> Courses Menu
                case 0:
                    menus[CurrentMenu].SetActive(false);
                    CurrentMenu = Menus.COURSES;
                    menus[CurrentMenu].SetActive(true);
                    break;
                // Dance -> Freeplay Menu
                case 1:
                    /*   menus[CurrentMenu].SetActive(false);
                       CurrentMenu = Menus.FREEPLAY;
                       menus[CurrentMenu].SetActive(true); */
                    break;
                default:
                    break;
            }
        }

        // TODO make this dynamic too
        private void SelectedInCoursesMenu(int selectedMenuOption)
        {
            switch (selectedMenuOption)
            {
                // Courses -> Salsa male Menu
                case 0:
                    menus[CurrentMenu].SetActive(false);
                    CurrentMenu = Menus.SPECCOURSE;
                    courseMenuHelper.LoadCourse("salsa");
                    menus[CurrentMenu].SetActive(true);
                    break;
                // Courses -> Salsa female Menu
                case 1:
                    break;
                default:
                    break;
            }
        }

        private void SelectedInSettingsMenu(int selectedMenuOption)
        {
            switch (selectedMenuOption)
            {
                // Settings -> AvatarSettings Menu
                case 0:
                    menus[CurrentMenu].SetActive(false);
                    CurrentMenu = Menus.AVATARSETTINGS;
                    menus[CurrentMenu].SetActive(true);
                    break;
                // Settings -> RGBFeedSettings Menu
                case 1:
                    
                    break;
                // Settings -> DifficultySettings Menu
                case 2:
                    
                    break;
                default:
                    break;
            }
        }

        private void SetTitleBarText()
        {
            
            switch (CurrentMenu)
            {
                case Menus.TITLE:
                    TitleText.text = "<size=120%>Motion Instructor</size>\nWhat are we going to do today?";
                    break;
                case Menus.DANCE:
                    TitleText.text = "<size=120%>Dance</size>\nLearn a new dance style, or challange yourself in one that you already know!";
                    break;
                case Menus.COURSES:
                    TitleText.text = "<size=120%>Courses</size>\nSelect the dance style you want to learn!";
                    break;
                case Menus.SPECCOURSE:
                    CourseInfoHolder info = courseMenuHelper.GetCurrentCourseInfo();
                    TitleText.text = "<size=120%>" + info.CourseTitle + "</size>\n" + info.CourseShortDescription;
                    break;
                case Menus.SETTINGS:
                    TitleText.text = "<size=120%>Settings</size>";
                    break;
                default:
                    break;
            }
        }

        public void SetCourseDetailsText(int courseID)
        {
            courseMenuHelper.SetCourseDetails(courseID);
        }
    }
}