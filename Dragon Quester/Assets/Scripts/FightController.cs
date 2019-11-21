using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class FightController : MonoBehaviour
{

    [SerializeField] Player Player1, Player2;
    Player.Command P1Command, P2Command;
    Entity p1, p2;
    [SerializeField] GameObject battleUIElements, victory;
    [SerializeField] Text P1_CommandText, P2_CommandText;
    public List<Spell> SpellList;
    [SerializeField] Transform TurnCount, attackTriange, P1Status, P2Status;
    [SerializeField] Camera mainCamera;
    Text TurnText;
    int turns, victor;
    
    [SerializeField] int playersReady = 0;

    bool battling, zoomCam;
    public bool someoneDied;

    [SerializeField]
    List<AnimationSequenceObject> animations = new List<AnimationSequenceObject>();

    private void Awake()
    {
        turns = 0;
        p1 = Player1.GetComponent<Entity>();
        p2 = Player2.GetComponent<Entity>();
        BeginBattle();
    }

    void BeginBattle()
    {
        zoomCam = true;
        TurnText = TurnCount.GetComponentInChildren<Text>();
        turns++;
        TurnText.text = "Turn " + turns;
        battling = true;
        playersReady = 0;
        Player1.StartBattle();
        Player2.StartBattle();
        
        attackTriange.DOLocalMoveY(0.0f, 1.0f, true);
    }

    void ReceiveCommand()
    {
        Debug.Log("command get");
        playersReady++;
        if(playersReady == 2)
        {
            StartCoroutine(BeginTurn());
        }
    }

    IEnumerator BeginTurn()
    {
        yield return new WaitForSeconds(0.5f);
        TweenBattleUIIn();
        StartCoroutine(TweenCameraIn());
        TweenPlayerStatusIn();
        TweenTurnUIOut();
        attackTriange.DOLocalMoveY(-720.0f, 0.6f, true);
        yield return new WaitForSeconds(0.8f);
        P1Command = Player1.GetAction();
        P2Command = Player2.GetAction();

        StartCoroutine(AttackChecks());
    }

    void EndTurn()
    {
        StartCoroutine(TweenCameraOut());
        TweenPlayerStatusOut();
        playersReady = 0;
        if (battling)
        {
            Player1.EndTurn(animations[5], p1, animations[7]);
            Player2.EndTurn(animations[5], p2, animations[7]);
            turns++;
            TurnText.text = "Turn " + turns;
            if (Player1.GetHealth() > 0 && Player2.GetHealth() > 0)
            {
                TweenTurnUIIn();
            }
        }
        else
        {
            // victory stuff
        }
    }

    public void PlayerDied(int whomst, bool immediate)
    {
        Debug.Log("now " + immediate);
        zoomCam = false;
        EndBattle();
        if (!someoneDied)
        {
            Debug.Log("a" + whomst);
            someoneDied = true;
            switch (whomst)
            {
                case 0:
                    victory.GetComponentInChildren<Text>().text = "Player 2 Wins!";
                    StartAnimationSequence(animations[6], p1, p2);
                    victor = 2;
                    break;
                case 1:
                    Debug.Log(1);
                    victory.GetComponentInChildren<Text>().text = "Player 1 Wins!";
                    StartAnimationSequence(animations[6], p2, p1);
                    victor = 1;
                    break;
            }
        }
        else {
            victory.GetComponentInChildren<Text>().text = "Draw!";
            Debug.Log("nobody");
            victor = 0;
            StartAnimationSequence(animations[6], p1, p2);
            StartAnimationSequence(animations[6], p2, p1);
            zoomCam = true;
        }
        if (!immediate)
        {
            StartCoroutine(EndGameTimer());
        }
        else
        {
            StartCoroutine(EndGameTimerImmediate());
        }
    }

    void EndBattle()
    {
        battling = false;
        Player1.HideUIGameEnd();
        Player2.HideUIGameEnd();
        TurnCount.gameObject.SetActive(false);
    }

    IEnumerator EndTurnTimer()
    {

        yield return new WaitForSeconds(1.5f);
        TweenBattleUIOut();
        yield return new WaitForSeconds(0.5f);
        EndTurn();
    }

    IEnumerator EndGameTimer()
    {
        yield return new WaitForSeconds(2.0f);
        TweenVictoryUIIn();
        TweenTurnUIOut();
        EndBattle();

        switch (victor)
        {
            case 1:
                Player1_Victory();
                break;
            case 2:
                Player2_Victory();
                break;
            default:
                break;
        }

        GameObject mp = GameObject.Find("MusicPlayer");
        if (mp != null)
            mp.SetActive(false);
        SoundManager.Instance.PlaySound("Sounds/HOES_MAD");

        yield return new WaitForSeconds(6.0f);
        SceneManager.LoadScene(0);
    }

    IEnumerator EndGameTimerImmediate()
    {
        TweenVictoryUIIn();
        TweenTurnUIOut();
        EndBattle();

        yield return new WaitForSeconds(Time.deltaTime);
        switch (victor)
        {
            case 1:
                Debug.Log("player 1 win");
                Player1_Victory();
                break;
            case 2:
                Debug.Log("player 2 win");
                Player2_Victory();
                break;
            default:
                Debug.Log("nobody win");
                break;
        }
        Debug.Log("victor" + victor);

        GameObject mp = GameObject.Find("MusicPlayer");
        if (mp != null)
            mp.SetActive(false);
        SoundManager.Instance.PlaySound("Sounds/HOES_MAD");

        yield return new WaitForSeconds(6.0f);
        SceneManager.LoadScene(0);
    }

    public Spell GetRandomSpell()
    {
        return SpellList[Random.Range(0, SpellList.Count)];
    }

    public void RemoveSpell(Spell s)
    {
        SpellList.Remove(s);
    }

    public void AddSpell(Spell s)
    {
        SpellList.Add(s);
    }

    void ApplyEffect(Player p, Effect e)
    {
        p.AddEffect(e);
    }


    private void StartAnimationSequence(AnimationSequenceObject aso, Entity e1, Entity e2)
    {
        AnimationSequence seq = new AnimationSequence(aso, e1, e2);
        seq.SequenceStart();
        StartCoroutine(seq.SequenceLoop());
    }

    void TweenBattleUIIn()
    {
        P1_CommandText.text = "";
        P2_CommandText.text = "";
        battleUIElements.transform.DOLocalMoveY(-10.0f, 0.6f, true);
    }

    void TweenBattleUIOut()
    {
        battleUIElements.transform.DOLocalMoveY(375.0f, 0.6f, true);
    }

    void TweenVictoryUIIn()
    {
        victory.transform.DOLocalMoveY(390, 1.0f, true);
    }

    void TweenTurnUIOut()
    {
        TurnCount.DOLocalMoveY(600, 0.6f, true);
    }

    void TweenTurnUIIn()
    {
        TurnCount.DOLocalMoveY(450, 0.6f, true);
    }

    IEnumerator TweenCameraIn()
    {
        if (zoomCam)
        {
            mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, 2.5f, Time.deltaTime);
            yield return null;
            if (mainCamera.orthographicSize >= 4.1f)
            {
                StartCoroutine(TweenCameraIn());
            }
        }
    }

    IEnumerator TweenCameraOut()
    {
        if (zoomCam)
        {
            mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, 6.5f, Time.deltaTime);
            yield return null;
            if (mainCamera.orthographicSize <= 5.1f)
            {
                StartCoroutine(TweenCameraOut());
            }
        }
    }

    void Player1_Victory()
    {
        if (victor == 1)
        {
            StopCoroutine(TweenCameraIn());
            StopCoroutine(TweenCameraOut());
            StartCoroutine(CameraVictoryZoom());
            mainCamera.transform.DOLocalMoveX(-5.0f, 1.0f, false);
        }
    }

    void Player2_Victory()
    {
        if (victor == 2)
        {
            StopCoroutine(TweenCameraIn());
            StopCoroutine(TweenCameraOut());
            StartCoroutine(CameraVictoryZoom());
            mainCamera.transform.DOLocalMoveX(5.0f, 1.0f, false);
        }
    }

    IEnumerator CameraVictoryZoom()
    {
        mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, 2.5f, Time.deltaTime);
        yield return null;
        if (mainCamera.orthographicSize >= 3.1f)
        {
            StartCoroutine(CameraVictoryZoom());
        }
    }

    void TweenPlayerStatusIn()
    {
        P1Status.DOLocalMove(new Vector3(-800.0f, 230.0f, 0.0f), 0.6f, true);
        P2Status.DOLocalMove(new Vector3(800.0f, 230.0f, 0.0f), 0.6f, true);
    }

    void TweenPlayerStatusOut()
    {
        P1Status.DOLocalMove(new Vector3(-750.0f, 200.0f, 0.0f), 0.6f, true);
        P2Status.DOLocalMove(new Vector3(750.0f, 200.0f, 0.0f), 0.6f, true);
    }

    IEnumerator AttackChecks()
    {
        P1_CommandText.text = "";
        P2_CommandText.text = "";

        // Check what to do first based off of Player 1's Command
        // Then check do things based on what Player 2's command
        // Repeat this process for each of Player 1's possible commands
        switch (P1Command)
        {
            // Player 1 Attack
            case Player.Command.Attack:

                switch (P2Command)
                {
                    case Player.Command.Attack:                                                     // P1 Attack, P2 Attack
                        P1_CommandText.text = "Attack!";
                        P2_CommandText.text = "Attack!";

                        StartAnimationSequence(animations[0], p1, p2);
                        StartAnimationSequence(animations[0], p2, p1);

                        yield return new WaitForSeconds(1.0f);
                        Player1.TakeDamage(1);
                        Player2.TakeDamage(1);

                        StartAnimationSequence(animations[1], p1, p2);
                        StartAnimationSequence(animations[1], p2, p1);

                        P1_CommandText.text = "Deal 1 Damage!";
                        P2_CommandText.text = "Deal 1 Damage!";
                        Player1.RestoreMP(1);
                        Player2.RestoreMP(1);
                        break;

                    case Player.Command.Block:                                                     // P1 Attack, P2 Block
                        P1_CommandText.text = "Attack!";
                        P2_CommandText.text = "Block!";

                        StartAnimationSequence(animations[0], p1, p2);
                        StartAnimationSequence(animations[2], p2, p1);

                        yield return new WaitForSeconds(1.0f);

                        StartAnimationSequence(animations[3], p2, p1);

                        Player1.TakeDamage(Player2.GetDamage());
                        P1_CommandText.text = "Parried! No Damage!";
                        P2_CommandText.text = ("Deal " + Player2.GetDamage().ToString() + " Damage!");
                        Player2.RestoreMP(Player2.GetDamage() * 1.5f);
                        break;

                    case Player.Command.Spell:                                                     // P1 Attack, P2 Spell
                        P1_CommandText.text = "Attack!";
                        P2_CommandText.text = Player2.GetSpell().GetName();
                        Player2.DrainMP(Player2.GetSpell().GetCost());

                        StartAnimationSequence(animations[0], p1, p2);
                        StartAnimationSequence(animations[4], p2, p1);

                        yield return new WaitForSeconds(1.0f);
                        Player2.TakeDamage(Player1.GetDamage());
                        P1_CommandText.text = "Deal " + Player1.GetDamage() + " Damage!";
                        P2_CommandText.text = "Interrupted!";
                        Player1.RestoreMP(Player1.GetDamage());

                        StartAnimationSequence(animations[1], p1, p2);
                        break;

                    case Player.Command.None:                                                     // P1 Attack, P2 Nothing
                        P1_CommandText.text = "Attack!";
                        P2_CommandText.text = "Can't Act!";

                        StartAnimationSequence(animations[0], p1, p2);

                        yield return new WaitForSeconds(1.0f);

                        StartAnimationSequence(animations[1], p1, p2);

                        Player2.TakeDamage(Player1.GetDamage());
                        P1_CommandText.text = ("Deal " + Player1.GetDamage().ToString() + " Damage!");
                        Player1.RestoreMP(Player1.GetDamage());
                        break;
                }

                break;

            // Player 1 Block
            case Player.Command.Block:

                switch (P2Command)
                {
                    case Player.Command.Attack:                                                     // P1 Block, P2 Attack
                        P1_CommandText.text = "Block!";
                        P2_CommandText.text = "Attack!";

                        StartAnimationSequence(animations[2], p1, p2);
                        StartAnimationSequence(animations[0], p2, p1);

                        yield return new WaitForSeconds(1.0f);

                        StartAnimationSequence(animations[3], p1, p2);

                        Player2.TakeDamage(Player1.GetDamage());
                        P1_CommandText.text = ("Deal " + Player1.GetDamage().ToString() + " Damage!");
                        P2_CommandText.text = ("Parried! No Damage!");
                        Player1.RestoreMP(Player1.GetDamage() * 1.5f);
                        break;

                    case Player.Command.Block:                                                     // P1 Block, P2 Block
                        P1_CommandText.text = "Block!";
                        P2_CommandText.text = "Block!";

                        StartAnimationSequence(animations[2], p1, p2);
                        StartAnimationSequence(animations[2], p2, p1);

                        yield return new WaitForSeconds(1.0f);
                        P1_CommandText.text = "wow FUCKING nothing";
                        P2_CommandText.text = "wow FUCKING nothing";
                        break;

                    case Player.Command.Spell:                                                     // P1 Block, P2 Spell
                        P1_CommandText.text = "Block!";
                        P2_CommandText.text = Player2.GetSpell().GetName();
                        Player2.DrainMP(Player2.GetSpell().GetCost());

                        StartAnimationSequence(animations[2], p1, p2);
                        StartAnimationSequence(animations[4], p2, p1);

                        yield return new WaitForSeconds(1.0f);
                        if (Player1.GetInvincible() && Player2.GetSpell().GetName() == "Metal Slash")
                        {
                            Player1.MakeVincible();
                            Player1.TakeDamage(100.0f);
                            P1_CommandText.text = "Shattered!";
                            P2_CommandText.text = "Max Damage!";

                            StartAnimationSequence(animations[8], p1, p2);
                        }
                        else
                        {
                            Player1.TakeDamage(Player2.GetSpell().GetDamage());
                            P1_CommandText.text = "wow FUCKING nothing";
                            P2_CommandText.text = Player2.GetSpell().GetDescription();
                            if (Player2.GetSpell().GetEffect() != null)
                            {
                                switch (Player2.GetSpell().GetEffect().onSelf)
                                {
                                    case true:
                                        ApplyEffect(Player2, Player2.GetSpell().GetEffect());
                                        break;
                                    case false:
                                        if(!Player1.GetInvincible()) ApplyEffect(Player1, Player2.GetSpell().GetEffect());
                                        break;
                                }
                            }
                        }

                        StartAnimationSequence(Player2.GetSpell().GetAnimation(), p2, p1);
                        break;

                    case Player.Command.None:                                                     // P1 Block, P2 Nothing
                        P1_CommandText.text = "Block!";
                        P2_CommandText.text = "Can't Act!";
                        StartAnimationSequence(animations[2], p1, p2);
                        yield return new WaitForSeconds(1.0f);
                        P1_CommandText.text = "wow FUCKING nothing";
                        break;
                }

                break;

            // Player 1 Spell
            case Player.Command.Spell:

                switch (P2Command)
                {
                    case Player.Command.Attack:                                                     // P1 Spell, P2 Attack
                        P1_CommandText.text = Player1.GetSpell().GetName();
                        P2_CommandText.text = "Attack!";
                        Player1.DrainMP(Player1.GetSpell().GetCost());

                        StartAnimationSequence(animations[4], p1, p2);
                        StartAnimationSequence(animations[0], p2, p1);

                        yield return new WaitForSeconds(1.0f);
                        Player1.TakeDamage(Player2.GetDamage());
                        P1_CommandText.text = ("Interrupted!");
                        P2_CommandText.text = ("Deal " + Player2.GetDamage().ToString() + " Damage!");
                        Player2.RestoreMP(Player2.GetDamage());

                        StartAnimationSequence(animations[1], p2, p1);
                        break;

                    case Player.Command.Block:                                                     // P1 Spell, P2 Block
                        P1_CommandText.text = Player1.GetSpell().GetName();
                        P2_CommandText.text = "Block!";
                        Player1.DrainMP(Player1.GetSpell().GetCost());

                        StartAnimationSequence(animations[4], p1, p2);
                        StartAnimationSequence(animations[2], p2, p1);

                        yield return new WaitForSeconds(1.0f);
                        if (Player2.GetInvincible() && Player1.GetSpell().GetName() == "Metal Slash")
                        {
                            Player2.MakeVincible();
                            Player2.TakeDamage(100.0f);
                            P2_CommandText.text = "Shattered!";
                            P1_CommandText.text = "Max Damage!";

                            StartAnimationSequence(animations[8], p2, p1);
                        }
                        else
                        {
                            Player2.TakeDamage(Player1.GetSpell().GetDamage());
                            P1_CommandText.text = Player1.GetSpell().GetDescription();
                            P2_CommandText.text = "wow FUCKING nothing";
                            if (Player1.GetSpell().GetEffect() != null)
                            {
                                switch (Player1.GetSpell().GetEffect().onSelf)
                                {
                                    case true:
                                        ApplyEffect(Player1, Player1.GetSpell().GetEffect());
                                        break;
                                    case false:
                                        if (!Player2.GetInvincible()) ApplyEffect(Player2, Player1.GetSpell().GetEffect());
                                        break;
                                }
                            }
                        }
                        StartAnimationSequence(Player1.GetSpell().GetAnimation(), p1, p2);
                        break;

                    case Player.Command.Spell:                                                     // P1 Spell, P2 Spell
                        P1_CommandText.text = Player1.GetSpell().GetName();
                        P2_CommandText.text = Player2.GetSpell().GetName();
                        Player1.DrainMP(Player1.GetSpell().GetCost());
                        Player2.DrainMP(Player2.GetSpell().GetCost());

                        StartAnimationSequence(animations[4], p1, p2);
                        StartAnimationSequence(animations[4], p2, p1);

                        yield return new WaitForSeconds(1.0f);

                        if (Player1.GetInvincible() && Player2.GetInvincible() && Player1.GetSpell().GetName() == "Metal Slash" && Player2.GetSpell().GetName() == "Metal Slash")
                        {
                            Player1.MakeVincible();
                            Player2.MakeVincible();
                            Player1.TakeDamage(100.0f);
                            Player2.TakeDamage(100.0f);
                            P1_CommandText.text = "Max Damage";
                            P2_CommandText.text = "Max Damage!";

                            StartAnimationSequence(animations[8], p1, p2);
                            StartAnimationSequence(animations[8], p2, p1);
                        }
                        else if (Player1.GetInvincible() || Player2.GetInvincible())
                        {
                            if (Player2.GetInvincible() && Player1.GetSpell().GetName() == "Metal Slash")
                            {
                                Player2.MakeVincible();
                                Player2.TakeDamage(100.0f);
                                P2_CommandText.text = "Shattered!";
                                P1_CommandText.text = "Max Damage!";

                                StartAnimationSequence(animations[8], p2, p1);
                            }
                            if (Player1.GetInvincible() && Player2.GetSpell().GetName() == "Metal Slash")
                            {
                                Player1.MakeVincible();
                                Player1.TakeDamage(100.0f);
                                P1_CommandText.text = "Shattered!";
                                P2_CommandText.text = "Max Damage!";

                                StartAnimationSequence(animations[8], p1, p2);
                            }
                        }
                        else
                        {
                            Player1.TakeDamage(Player2.GetSpell().GetDamage());
                            Player2.TakeDamage(Player1.GetSpell().GetDamage());
                            P1_CommandText.text = Player1.GetSpell().GetDescription();
                            P2_CommandText.text = Player2.GetSpell().GetDescription();
                            if (Player1.GetSpell().GetEffect() != null)
                            {
                                switch (Player1.GetSpell().GetEffect().onSelf)
                                {
                                    case true:
                                        ApplyEffect(Player1, Player1.GetSpell().GetEffect());
                                        break;
                                    case false:
                                        if (!Player2.GetInvincible()) ApplyEffect(Player2, Player1.GetSpell().GetEffect());
                                        break;
                                }
                            }
                            if (Player2.GetSpell().GetEffect() != null)
                            {
                                switch (Player2.GetSpell().GetEffect().onSelf)
                                {
                                    case true:
                                        ApplyEffect(Player2, Player2.GetSpell().GetEffect());
                                        break;
                                    case false:
                                        if (!Player1.GetInvincible()) ApplyEffect(Player1, Player2.GetSpell().GetEffect());
                                        break;
                                }
                            }
                        }
                        StartAnimationSequence(Player1.GetSpell().GetAnimation(), p1, p2);
                        StartAnimationSequence(Player2.GetSpell().GetAnimation(), p2, p1);
                        break;

                    case Player.Command.None:                                                     // P1 Spell, P2 Nothing
                        P1_CommandText.text = Player1.GetSpell().GetName();
                        Player1.DrainMP(Player1.GetSpell().GetCost());
                        P2_CommandText.text = "Can't Act!";

                        StartAnimationSequence(animations[4], p1, p2);

                        yield return new WaitForSeconds(1.0f);
                        if (Player2.GetInvincible() && Player1.GetSpell().GetName() == "Metal Slash")
                        {
                            Player2.MakeVincible();
                            Player2.TakeDamage(100.0f);
                            P2_CommandText.text = "Shattered!";
                            P1_CommandText.text = "Max Damage!";

                            StartAnimationSequence(animations[8], p2, p1);
                        }
                        else
                        {
                            Player2.TakeDamage(Player1.GetSpell().GetDamage());
                            P1_CommandText.text = Player1.GetSpell().GetDescription();
                            if (Player1.GetSpell().GetEffect() != null)
                            {
                                switch (Player1.GetSpell().GetEffect().onSelf)
                                {
                                    case true:
                                        ApplyEffect(Player1, Player1.GetSpell().GetEffect());
                                        break;
                                    case false:
                                        if (!Player2.GetInvincible()) ApplyEffect(Player2, Player1.GetSpell().GetEffect());
                                        break;
                                }
                            }
                        }

                        StartAnimationSequence(Player1.GetSpell().GetAnimation(), p1, p2);
                        break;
                }

                break;

            // Player 1 Nothing
            case Player.Command.None:
                P1_CommandText.text = "Can't Act!";
                switch (P2Command)
                {
                    case Player.Command.Attack:                                                     // P1 Nothing, P2 Attack
                        P2_CommandText.text = "Attack!";

                        StartAnimationSequence(animations[0], p2, p1);

                        yield return new WaitForSeconds(1.0f);
                        Player1.TakeDamage(Player2.GetDamage());
                        P2_CommandText.text = ("Deal " + Player2.GetDamage().ToString() + " Damage!");
                        Player2.RestoreMP(Player2.GetDamage());

                        StartAnimationSequence(animations[1], p2, p1);
                        break;

                    case Player.Command.Block:                                                     // P1 Nothing, P2 Block
                        P2_CommandText.text = "Block!";

                        StartAnimationSequence(animations[2], p2, p1);

                        yield return new WaitForSeconds(1.0f);
                        P2_CommandText.text = "wow FUCKING nothing";
                        break;

                    case Player.Command.Spell:                                                     // P1 Nothing, P2 Spell
                        P2_CommandText.text = Player2.GetSpell().GetName();
                        Player2.DrainMP(Player2.GetSpell().GetCost());

                        StartAnimationSequence(animations[4], p2, p1);

                        yield return new WaitForSeconds(1.0f);
                        if (Player1.GetInvincible() && Player2.GetSpell().GetName() == "Metal Slash")
                        {
                            Player1.MakeVincible();
                            Player1.TakeDamage(100.0f);
                            P1_CommandText.text = "Shattered!";
                            P2_CommandText.text = "Max Damage!";

                            StartAnimationSequence(animations[8], p1, p2);
                        }
                        else
                        {
                            Player1.TakeDamage(Player2.GetSpell().GetDamage());
                            P2_CommandText.text = Player2.GetSpell().GetDescription();
                            if (Player2.GetSpell().GetEffect() != null)
                            {
                                switch (Player2.GetSpell().GetEffect().onSelf)
                                {
                                    case true:
                                        ApplyEffect(Player2, Player2.GetSpell().GetEffect());
                                        break;
                                    case false:
                                        if (!Player1.GetInvincible()) ApplyEffect(Player1, Player2.GetSpell().GetEffect());
                                        break;
                                }
                            }
                        }
                        StartAnimationSequence(Player2.GetSpell().GetAnimation(), p2, p1);
                        break;

                    case Player.Command.None:                                                     // P1 Nothing, P2 Nothing
                        P2_CommandText.text = "Can't Act!";
                        yield return new WaitForSeconds(1.0f);
                        break;
                }

                break;
        }
        StartCoroutine(EndTurnTimer());
    }
}
