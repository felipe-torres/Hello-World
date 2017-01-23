using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using DG.Tweening;
using System.Linq;

/// <summary>
///
/* _
 _._ _..._ .-',     _.._(`))
'-. `     '  /-._.-'    ',/
   )         \            '.
  / _    _    |             \
 |  a    a    /              |
 \   .-.                     ;
  '-('' ).-'       ,'       ;
     '-;           |      .'
        \           \    /
        | 7  .__  _.-\   \
        | |  |  ``/  /`  /
       /,_|  |   /,_/   /
          /,_/      '`-'
GGJ2017
*/
/// </summary>

public class MainManager : MonoBehaviour
{
    public Elevator[] Elevators;
    public TextAsset xmlLevelConfig;
    public int personajesPegados;

    private GameManager gm;
    public Player Player;

    /// <summary>
    /// Positions of enemies on de elevator
    /// </summary>

    Vector3[] positionsCitizensOnElevator;

    /// <summary>
    /// Control variables
    /// </summary>

    public GameObject Enemy;
    public int ActualWave;
    public int ActualCountEnemies;
    List<LevelConfig> listaLevels;

    public Camera camara;
    Animator anim;

    public GameObject CreditsPanel;

    private bool HasGameOverStarted = false;
    private bool DidPlayerWon = false;

    public GameObject WinObject;
    public GameObject LostObject;

    void Awake()
    {
        gm = GameManager.Instance;
    }

    void Start()
    {
        ActualCountEnemies = 0;
        ActualWave = 1;
        positionsCitizensOnElevator = new Vector3[9];
        positionsCitizensOnElevator[0] = new Vector3(128f, 78.07f, -193f);
        positionsCitizensOnElevator[1] = new Vector3(7f, 78.07f, -193f);
        positionsCitizensOnElevator[2] = new Vector3(-123f, 78.07f, -193);
        positionsCitizensOnElevator[3] = new Vector3(-107f, 78.07f, -341f);
        positionsCitizensOnElevator[4] = new Vector3(-5f, -78.07f, -341f);
        positionsCitizensOnElevator[5] = new Vector3(126f, 78.07f, -341f);
        positionsCitizensOnElevator[6] = new Vector3(-83f, 78.07f, -445f);
        positionsCitizensOnElevator[7] = new Vector3(46f, 78.07f, -445f);
        positionsCitizensOnElevator[8] = new Vector3(139f, 78.07f, -445f);
        LoadData();

        anim = camara.GetComponent<Animator>();
        gm.OnStateChange += HandleOnStateChange;
        gm.SetGameState(GameState.MainMenu);
    }

    public void HandleOnStateChange()
    {
        switch (gm.gameState)
        {
        case GameState.SplashScreen:
            break;
        case GameState.GamePlay:
            camara.transform.DOMove(new Vector3(-38.5f, 51.4f, -40f), 1.5f).SetEase(Ease.InOutQuad);
            camara.DOOrthoSize(9.5f, 1.5f).SetEase(Ease.InOutQuad);
            StartCoroutine(MainLoopSequence());
            AudioManager.Instance.AddNextChannel();
            GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().CanMove = true;
            break;
        case GameState.GameOver:
            GameOver();
            break;
        case GameState.Pausa:
            break;
        case GameState.Tutorial:
            break;
        case GameState.MainMenu:
            camara.transform.DOMove(new Vector3(-38.5f, -30.8f, -40f), 0f);
            camara.DOOrthoSize(20.32f, 0f);
            break;
        }


        print("Changed to: " + gm.gameState);
    }

    private void CheckGameOver()
    {
        if (ActualWave > listaLevels.Count)
        {
            // Won
            gm.SetGameState(GameState.GameOver);
            DidPlayerWon = true;
        }
        else if (Player.EnemiesStuck >= personajesPegados)
        {
            // Lost
            gm.SetGameState(GameState.GameOver);
        }
    }

    private void Update()
    {
        if (!HasGameOverStarted) CheckGameOver();
    }

    private void InstantiateEnemy(int elevatorIndex)
    {
        GameObject enemy = Instantiate(Enemy) as GameObject;
        enemy.transform.SetParent(Elevators[elevatorIndex].CitizenParent.transform, false);
        enemy.transform.localPosition = positionsCitizensOnElevator[Random.Range(0, 9)];
        enemy.GetComponent<Citizen>().TargetPlayer = GameObject.FindGameObjectWithTag("Player").transform;
        ActualCountEnemies++;
    }

    IEnumerator MainLoopSequence()
    {
        yield return new WaitForSeconds(2f);

        print("Starting game of " + listaLevels.Count + " levels");

        int elevatorIndex = 0;

        while (!HasGameOverStarted) // win or loose condition
        {
            yield return new WaitForSeconds(3f);

            int maxEnemies = listaLevels.ElementAt(ActualWave - 1).citizens;
            int elevatorsToUse = Random.Range(1, 5);

            print(elevatorsToUse + " elevators to use");

            if (ActualCountEnemies < maxEnemies)
            {
                for (int i = 0; i < elevatorsToUse; i++) // Number of elevators to spawn
                {
                    StartCoroutine(ElevatorSequence());
                    yield return new WaitForSeconds(1f);
                }
            }
            else
            {
                ActualWave++;
                ActualCountEnemies = 0;

                print("Starts actual wave " + ActualWave);

                if (ActualWave == 2 || ActualWave == 4)
                {
                    AudioManager.Instance.AddNextChannel();
                }
            }

            yield return new WaitForSeconds(3f);
        }

    }

    private IEnumerator ElevatorSequence()
    {
        List<Elevator> FreeElevators;
        FreeElevators = GetFreeElevators();

        while (FreeElevators.Count <= 0) yield return null;

        int elevatorIndex = Random.Range(0, FreeElevators.Count);

        //print("Bug pls: " + FreeElevators.Count + " , " + elevatorIndex);
        FreeElevators[elevatorIndex].IsFree = false;

        for (int j = 0; j < Random.Range(1, 9); j ++)
        {
            InstantiateEnemy(elevatorIndex); // Instantiate at elevator 0
        }

        yield return new WaitForSeconds(1f);
        Elevators[elevatorIndex].Ascend();

        yield return new WaitForSeconds(4f);
        Elevators[elevatorIndex].Descend();
    }

    private List<Elevator> GetFreeElevators()
    {
        List<Elevator> FreeElevators = new List<Elevator>();
        foreach (Elevator e in Elevators)
        {
            if (e.IsFree)
            {
                FreeElevators.Add(e);
            }
        }

        return FreeElevators;
    }

    void LoadData()
    {
        listaLevels = new List<LevelConfig>();

        if (xmlLevelConfig != null)
        {

            XDocument xDoc = XDocument.Parse(xmlLevelConfig.text);

            listaLevels = (from item in xDoc.Descendants("wave") select new LevelConfig()
            {
                id = int.Parse(item.Element("id").Value),
                citizens = int.Parse(item.Element("citizens").Value)
            }).ToList();
        }

        else
        {
            Debug.Log("No puedo acceder al archivo");
        }
    }


    public void Credits()
    {
        CreditsPanel.gameObject.SetActive(true);
        AudioManager.Instance.PlaySFX(AudioManager.SFXType.o3);

        camara.transform.DOMove(new Vector3(-8.8f, -30.8f, -40f), 1.0f).SetEase(Ease.InOutQuad);
    }

    public void Play()
    {
        gm.SetGameState(GameState.GamePlay);
        AudioManager.Instance.PlaySFX(AudioManager.SFXType.o2);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void GameOver()
    {
        StartCoroutine(GameOverSequence());
    }

    public void Back()
    {
        camara.transform.DOMove(new Vector3(-38.5f, -30.8f, -40f), 1f);
        camara.DOOrthoSize(20.32f, 1f);
        ActualWave = 1;
        Player.CanMove = false;
        Player.transform.position = Vector3.zero;
        Player.speed = 2f;
        
        foreach(GameObject g in GameObject.FindGameObjectsWithTag("Citizen"))
        {
            Destroy(g);
        }
        Player.EnemiesStuck = 0;
        HasGameOverStarted = false;
        gm.SetGameState(GameState.MainMenu);

        foreach (Elevator e in Elevators) 
        {
            e.Descend();
        }

    }

    private IEnumerator GameOverSequence()
    {
        Player.CanMove = false;
        HasGameOverStarted = true;
        yield return new WaitForSeconds(1f);
        camara.transform.DOMoveY(76.85f, 2f);

        if (DidPlayerWon)
        {
            print("Won");
            WinObject.SetActive(true);
            LostObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            WinObject.GetComponent<Animator>().SetTrigger("activate");
        }
        else
        {
            print("Lost");
            WinObject.SetActive(false);
            LostObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            LostObject.GetComponent<Animator>().SetTrigger("activate");
        }
    }

}
