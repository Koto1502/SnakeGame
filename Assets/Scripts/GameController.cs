using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController instance = null;

    const float width = 3.7f;
    const float height = 7f;

    public BodyPart bodyPrefab = null;
    public GameObject rockPrefab = null;
    public GameObject eggPrefab = null;
    public GameObject goldEggPrefab = null;
    public GameObject spikePrefab = null;

    public bool alive = true;

    public float snakeSpeed = 3;

    public Sprite tailSprite = null;
    public Sprite bodyPartSprite = null;

    public SnakeHead snakeHead = null;

    public bool waitingToPlay = true;

    List<Egg> eggs = new List<Egg>();
    List<Spike> spikes = new List<Spike>();

    [SerializeField] int level = 0;
    int noOfEggsForNextLevel = 0;

    public int score = 0;
    public int hiScore = 0;

    public Text scoreText = null;
    public Text hiScoreText = null;

    public Text gameOverText = null;
    public Text tapToPlayText = null;

    public Text levelText = null;

    public GameObject BgMusic = null;
    bool musicOn = true;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        CreateWalls();
        alive = false;
        hiScore = PlayerPrefs.GetInt("hiScore");
    }

    public void GameOver()
    {
        level = 0;
        alive = false;
        waitingToPlay = true;



        tapToPlayText.gameObject.SetActive(true);
        gameOverText.gameObject.SetActive(true);
    }

    void StartGameplay()
    {
        score = 0;

        scoreText.text = "Score = " + score;
        hiScoreText.text = "HiScore = " + hiScore;

        gameOverText.gameObject.SetActive(false);
        tapToPlayText.gameObject.SetActive(false);

        waitingToPlay = false;
        alive = true;

        KillOldEggs();
        KillOldSpikes();
        LevelUp();
    }

    void LevelUp()
    {
        level++;
        levelText.text = "Level " + level;

        noOfEggsForNextLevel = (level * 2) + 4;

        snakeSpeed = 1.5f + (level / 4f);
        if (snakeSpeed > 6) snakeSpeed = 6;

        snakeHead.ResetSnake();
        CreateEgg();
        KillOldSpikes();
        CreateSpikes();

    }
    public void EggEaten(Egg egg)
    {
        score++;

        noOfEggsForNextLevel--;
        if (noOfEggsForNextLevel == 0)
        {
            score += 10;
            LevelUp();
        }

        else if (noOfEggsForNextLevel == 1)
            CreateEgg(true);
        else
            CreateEgg(false);


        if (score > hiScore)
        {
            hiScore = score;
            hiScoreText.text = "HiScore = " + hiScore;
            PlayerPrefs.SetInt("hiScore", hiScore);
        }

        scoreText.text = "Score = " + score;
        eggs.Remove(egg);
        Destroy(egg.gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        if (waitingToPlay)
        {
            foreach (Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Ended)
                {
                    StartGameplay();
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                StartGameplay();
            }
        }
    }


    void CreateWalls()
    {
        float z = -1;
        //need check
        Vector3 start = new Vector3(-width, -height, z);
        Vector3 finish = new Vector3(-width, +height, z);
        CreateWall(start, finish);

        start = new Vector3(+width, -height, z);
        finish = new Vector3(+width, +height, z);
        CreateWall(start, finish);

        start = new Vector3(-width, -height, z);
        finish = new Vector3(+width, -height, z);
        CreateWall(start, finish);

        start = new Vector3(-width, +height, z);
        finish = new Vector3(+width, +height, z);
        CreateWall(start, finish);
    }

    void CreateWall(Vector3 start, Vector3 finish)
    {
        //check Z
        float distance = Vector3.Distance(start, finish);
        int noOfRocks = (int)(distance * 3f);
        Vector3 delta = (finish - start) / noOfRocks;

        Vector3 position = start;
        for (int rock = 0; rock <= noOfRocks; rock++)
        {
            float rotation = Random.Range(0, 360f);
            float scale = Random.Range(1.5f, 2f);
            CreateRock(position, scale, rotation);
            position += delta;
        }
    }

    void CreateRock(Vector3 position, float scale, float rotation)
    {
        //check Z
        GameObject rock = Instantiate(rockPrefab, position, Quaternion.Euler(0, 0, rotation));
        rock.transform.localScale = new Vector3(scale, scale, 1);
    }

    void CreateEgg(bool golden = false)
    {
        //check Z
        Vector3 position;
        position.x = -width + Random.Range(1f, (width * 2) - 2f);
        position.y = -height + Random.Range(1f, (height * 2) - 2f);
        position.z = -1.1f;
        Egg egg = null;
        if (golden)
        {
            egg = Instantiate(goldEggPrefab, position, Quaternion.identity).GetComponent<Egg>();
        }
        else
        {
            egg = Instantiate(eggPrefab, position, Quaternion.identity).GetComponent<Egg>();
        }
        eggs.Add(egg);
    }
    void KillOldEggs()
    {
        foreach (Egg egg in eggs)
        {
            Destroy(egg.gameObject);
        }
        eggs.Clear();
    }

    void CreateSpikes()
    {
        for (int i = 0; i < level * 4; i++)
        {
            CreateSpike();
        }
    }
    void CreateSpike()
    {
        int[] toSide = new int[2] { 1, -1 };
        //check Z
        Vector3 position;
        position.x = Random.Range(0.3f, width - 1f) * toSide[Random.Range(0, toSide.Length)];
        position.y = Random.Range(0.3f, height - 1f) * toSide[Random.Range(0, toSide.Length)];
        position.z = -1.2f;
        Spike spike = null;
        spike = Instantiate(spikePrefab, position, Quaternion.identity).GetComponent<Spike>();
        spikes.Add(spike);
    }

    void KillOldSpikes()
    {
        foreach (Spike spike in spikes)
        {
            Destroy(spike.gameObject);
        }
        spikes.Clear();
    }

    public void toggleMusic()
    {
        AudioSource bgMusic = BgMusic.GetComponent<AudioSource>();
        if (musicOn)
        {
            bgMusic.Pause();
            musicOn = false;
        }
        else
        {
            bgMusic.Play();
            musicOn = true;
        }
    }
}
