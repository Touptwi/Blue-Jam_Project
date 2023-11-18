using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{

    // GameManager things
    public static GameManager Instance { get; private set; }
    void Awake()
    {
        if (Instance != null)
            Destroy(Instance);
        Instance = this;
    }
 
    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    // (Global) variables
    private float happyness, city, economy, paperwork, defaultValue, minValue, maxValue;
    private List<Event> listEvents;

    // Event which is hapenning
    public Event  currentEvent;
    public bool needUIUpdate;

    // Init Stats at the begenning
    private void InitStats() 
    {
        this.minValue = 0.0f;
        this.maxValue = 1.0f;
        this.defaultValue = 0.5f;
        this.happyness = defaultValue;
        this.city = defaultValue;
        this.economy = defaultValue;
        this.paperwork = defaultValue;
    }

    public float CalculateScore() 
    {
        return this.happyness*this.city*this.economy*this.paperwork*400;
    }

    // Init some events
    private void InitEvents() 
    {
        this.listEvents = new List<Event>();

        AddEvent(new Event("Eval", "Est-ce que tu aimes le jeu ?", 
                new List<Choice>(){ new Choice("Oui", 0.0f, 0.3f, 0.0f, 0.0f), 
                                    new Choice("Non", 0.0f, -0.3f, 0.0f, 0.0f)}));
        AddEvent(new Event("Gen", "Pourquoi notre génération pue ?", 
                new List<Choice>(){ new Choice("42", 0.0f, 0.3f, 0.0f, 0.0f), 
                                    new Choice("Women", 0.0f, -0.3f, 0.0f, 0.0f)}));
        AddEvent(new Event("Avenir", "T'as un rêve ou un objectif ?", 
                new List<Choice>(){ new Choice("Objectif", 0.0f, 0.3f, 0.0f, 0.0f), 
                                    new Choice("Rêve", 0.0f, -0.3f, 0.0f, 0.0f)}));
        AddEvent(new Event("Ennuie", "Tu préfères manger ou dormir ?", 
                new List<Choice>(){ new Choice("Manger", 0.0f, 0.3f, 0.0f, 0.0f), 
                                    new Choice("Dormir", 0.0f, -0.3f, 0.0f, 0.0f)}));
    }

    // Events Functions
    public void AddEvent(Event Event) { this.listEvents.Add(Event); Debug.Log("Added the event: " + Event.GetTitle()); }
    public void RemoveEvent(Event Event) { this.listEvents.Remove(Event); Debug.Log("Removed the event: " + Event.GetTitle()); }
    public void PickRandomEvent() 
    { 
        if(this.listEvents.Count > 0)
        {
            this.currentEvent = this.listEvents[Random.Range(0,this.listEvents.Count-1)]; 
            //Debug Info about Event selected
            string choiceString = this.currentEvent.GetTitle() + "\n"
                                + this.currentEvent.GetDesc() + "\n";
            foreach(Choice choice in this.currentEvent.GetChoices())
            {
                choiceString += choice.GetText() + " ";
            }
            Debug.Log("Event selected: " + choiceString);
        }
        else
            this.currentEvent = null;

        this.needUIUpdate = true;
    }

    // Add value to stats (happyness, city, economy and paperwork)
    public void AddStats(float Happyness, float City, float Economy, float Paperwork) 
    {
        this.happyness = Mathf.Clamp(happyness+Happyness, minValue, maxValue);
        this.city = Mathf.Clamp(city+City, minValue, maxValue);
        this.economy = Mathf.Clamp(economy+Economy, minValue, maxValue);
        this.paperwork = Mathf.Clamp(paperwork+Paperwork, minValue, maxValue);
        Debug.Log("new stats: \n"
                + "     Happyness:" + this.happyness + "\n"
                + "     City:" + this.city + "\n"
                + "     Economy:" + this.economy + "\n"
                + "     Paperwork:" + this.paperwork);
    }

    // Update the Event Window with the new current Event
    public void UpdateEventUI() 
    {
        if(this.currentEvent != null)
        {
            GameObject.Find("Event Window/Description").GetComponent<TMP_Text>().text = this.currentEvent.GetDesc();
            GameObject.Find("Event Window/Choices/Choice1/Text (TMP)").GetComponent<TMP_Text>().text = this.currentEvent.GetChoices()[0].GetText();
            GameObject.Find("Event Window/Choices/Choice1").GetComponent<OnClickChoice>().ChoiceNumber = 0;
            GameObject.Find("Event Window/Choices/Choice2/Text (TMP)").GetComponent<TMP_Text>().text = this.currentEvent.GetChoices()[1].GetText();
            GameObject.Find("Event Window/Choices/Choice2").GetComponent<OnClickChoice>().ChoiceNumber = 1;
        }
        else
        {
            GameObject.Find("Event Window").SetActive(false);
            Debug.Log("END \n Your score is " + CalculateScore());
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        InitStats();
        InitEvents();
        PickRandomEvent();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if it needs to update UI
        if(this.needUIUpdate == true)
        {
            UpdateEventUI();
            this.needUIUpdate = false;
        }
        if(this.happyness*this.city*this.economy*this.paperwork == 0.0f)
        {
            Debug.Log("YOU LOST");
            GameObject.Find("Event Window").SetActive(false);
            GameObject.Find("GameManager").SetActive(false);
        }
    }
}

// Create an Event that can be added
public class Event
{
    // Event variables
    private string title, description;
    private List<Choice> choices;

    // Event constructor
    public Event(string Title, string Description, List<Choice> Choices)
    {
        this.title = Title;
        this.description = Description;
        this.choices = Choices;
    }

    // Getters
    public string GetTitle() { return this.title; }
    public string GetDesc() { return this.description; }
    public List<Choice> GetChoices() { return this.choices; }
}

// Create a choice to be selected or not by the user
public class Choice
{
    // Choice variables
    private string text;
    private float happyness, city, economy, paperwork;

    // Choice constructor
    public Choice(string Text, float Happyness, float City, float Economy, float Paperwork)
    {
        this.text = Text;
        this.happyness = Happyness;
        this.city = City;
        this.economy = Economy;
        this.paperwork = Paperwork;
    }

    // Getter
    public string GetText() { return this.text; }
    // Apply Effect from that choice which change stats
    public void ApplyEffect() 
    { 
        GameManager.Instance.AddStats(happyness, city, economy, paperwork); 
        Debug.Log("Effect Applied for " + this.text);
        GameManager.Instance.RemoveEvent(GameManager.Instance.currentEvent);
        GameManager.Instance.PickRandomEvent();
    }
}
