using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // Init some events
    private void InitEvents() 
    {
        this.listEvents = new List<Event>();

        AddEvent(new Event("Eval", "Est-ce que tu aimes le jeu ?", 
                new List<Choice>(){ new Choice("Oui", 1.0f, 1.0f, 1.0f, 1.0f), 
                                    new Choice("Non", -1.0f, -1.0f, -1.0f, -1.0f)}));
    }

    // Events Functions
    private void AddEvent(Event Event) { this.listEvents.Add(Event); Debug.Log("Added the event: " + Event.GetTitle()); }
    private void RemoveEvent(Event Event) { this.listEvents.Remove(Event); Debug.Log("Removed the event: " + Event.GetTitle()); }
    private void PickRandomEvent() { this.currentEvent = this.listEvents[Random.Range(0,this.listEvents.Count-1)]; }

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


    // Start is called before the first frame update
    void Start()
    {
        InitStats();
        InitEvents();
        PickRandomEvent();

        string choiceString = this.currentEvent.GetTitle() + "\n"
                            + this.currentEvent.GetDesc() + "\n";
        foreach(Choice choice in this.currentEvent.GetChoices())
        {
            choiceString += choice.GetText() + " ";
        }
        Debug.Log(choiceString);

        this.currentEvent.GetChoices()[0].ApplyEffect();
        this.currentEvent.GetChoices()[1].ApplyEffect();
    }

    // Update is called once per frame
    void Update()
    {
        
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
    public void ApplyEffect() { GameManager.Instance.AddStats(happyness, city, economy, paperwork); Debug.Log("Effect Applied for " + this.text); }
}
