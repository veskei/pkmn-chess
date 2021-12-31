using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PkmChess : MonoBehaviour
{

    public GameManager gm_script;
    public PieceStats ps_script;
    public Sprite[] pieces_white, pieces_black;
    public Sprite attack_indicator, def_indicator;
    public Image enemy_indicator;
    public PieceUnit player_piece, enemy_piece;
    // Piece stats

    // UI Elements
    public Image flasher;
    public GameObject battle_panel, pre_combat, combat_menu, attack_menu, attacking_menu;
    public Image player_piece_sprite, enemy_piece_sprite, player_pad, enemy_pad;
    public Text player_piece_text, player_health_text, enemy_piece_text;
    public Image player_health_sprite, enemy_health_sprite;
    public float player_health, player_max_health, enemy_health, enemy_max_health;
    public Text ability1, ability2, ability3, ability4;
    public Text combat_text, combat_text_shade;
    public Text attacking_text, attacking_text_shade;
    Piece white, black;
    public float slide_speed;
    int player_pad_x, enemy_pad_x;
    bool enemy_defend = false, player_defend = false;
    bool enemy_weak = false, player_weak = false;

    Ability abil, enemy_next_turn;

    // Start is called before the first frame update
    void Start()
    {
        flasher.CrossFadeAlpha(0f, 0f, false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GameStart(Piece a, Piece d)
    {
        StartCoroutine(Flashing(a, d));
    }

    public IEnumerator Flashing(Piece att, Piece def)
    {

        if (att.team == PieceTeam.White)
        {
            white = att;
            black = def;
        }
        else
        {
            white = def;
            black = att;
        }

        yield return new WaitForSeconds(0.3f);
        flasher.CrossFadeAlpha(.8f, 0f, false);
        yield return new WaitForSeconds(0.25f);
        flasher.CrossFadeAlpha(0f, 0f, false);
        yield return new WaitForSeconds(0.25f);
        flasher.CrossFadeAlpha(.8f, 0f, false);
        yield return new WaitForSeconds(0.25f);
        flasher.CrossFadeAlpha(0f, 0f, false);
        yield return new WaitForSeconds(0.25f);
        flasher.CrossFadeAlpha(1f, 0f, false);
        
        player_piece = ps_script.piece_stats[(int)white.type];
        enemy_piece = ps_script.piece_stats[(int)black.type];

        battle_panel.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        flasher.CrossFadeAlpha(0f, 0f, false);

        InitUI();
        StartCoroutine(Slider());

    }

    // Slides in pieces from sides to position
    public IEnumerator Slider()
    {
        for (int i = 1200; i > 150; i--)
        {
            player_pad.rectTransform.localPosition = new Vector2(i-450, player_pad.rectTransform.localPosition.y);
            enemy_pad.rectTransform.localPosition = new Vector2(-i + 450, enemy_pad.rectTransform.localPosition.y);
            yield return new WaitForSeconds(Time.deltaTime * slide_speed);
        }

        yield return new WaitForSeconds(0.3f);

        combat_menu.SetActive(true);
    }

    public void InitUI()
    {
        ability3.transform.parent.gameObject.SetActive(true);
        ability4.transform.parent.gameObject.SetActive(true);

        player_piece_sprite.sprite = pieces_white[(int)white.type - 1];
        enemy_piece_sprite.sprite = pieces_black[(int)black.type - 1];
        
        player_piece_text.text = player_piece.name.ToUpper();
        enemy_piece_text.text = enemy_piece.name.ToUpper();
        
        player_max_health = player_piece.health;
        player_health = player_max_health;

        enemy_max_health = enemy_piece.health;
        enemy_health = enemy_max_health;

        player_health_text.text = player_health + "/" + player_max_health;

        ability1.text = player_piece.abilities[0].ability_name.ToUpper();
        ability2.text = player_piece.abilities[1].ability_name.ToUpper();

        if (player_piece.abilities[2] != null)
            ability3.text = player_piece.abilities[2].ability_name.ToUpper();
        else
            ability3.transform.parent.gameObject.SetActive(false);
        
        if (player_piece.abilities[3] != null)
            ability4.text = player_piece.abilities[3].ability_name.ToUpper();
        else
            ability4.transform.parent.gameObject.SetActive(false);

        SetEnemyAbility();
        combat_text.text = "What should " + player_piece.name.ToUpper() + " do?";
        combat_text_shade.text = combat_text.text;
    }

    public void ShowAttackPanel()
    {
        attack_menu.SetActive(true);
    }

    public void UseAbility(int n)
    {
        StartCoroutine(PlayerAnim(n));
    }

    public void Run()
    {
        GameEnd(PieceTeam.Black);
    }

    public IEnumerator PlayerAnim(int n)
    {
        player_weak = false;
        player_defend = false;

        abil = player_piece.abilities[n];
        string t_text = player_piece.name.ToUpper() + " used " + abil.ability_name.ToUpper() + "!";
        attacking_menu.SetActive(true);

        for (int i = 0; i <= t_text.Length; i++)
        {
            attacking_text.text = t_text.Substring(0, i);
            attacking_text_shade.text = attacking_text.text;
            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitForSeconds(0.5f);

        if (!abil.defense)
        {
            for (int i = 0; i < 30; i++)
            {
                player_piece_sprite.rectTransform.localPosition = new Vector3(-i, player_piece_sprite.rectTransform.localPosition.y, 0);
                yield return new WaitForSeconds(Time.deltaTime);
            }
            yield return new WaitForSeconds(0.1f);
            player_piece_sprite.rectTransform.localPosition = new Vector3(5, player_piece_sprite.rectTransform.localPosition.y, 0);
            yield return new WaitForSeconds(0.1f);
            player_piece_sprite.rectTransform.localPosition = new Vector3(0, player_piece_sprite.rectTransform.localPosition.y, 0);

            yield return new WaitForSeconds(0.3f);
            enemy_piece_sprite.CrossFadeAlpha(0f, 0f, false);
            yield return new WaitForSeconds(0.08f);
            enemy_piece_sprite.CrossFadeAlpha(1f, 0f, false);
            yield return new WaitForSeconds(0.08f);
            enemy_piece_sprite.CrossFadeAlpha(0f, 0f, false);
            yield return new WaitForSeconds(0.08f);
            enemy_piece_sprite.CrossFadeAlpha(1f, 0f, false);
            yield return new WaitForSeconds(0.08f);
            enemy_piece_sprite.CrossFadeAlpha(0f, 0f, false);
            yield return new WaitForSeconds(0.08f);
            enemy_piece_sprite.CrossFadeAlpha(1f, 0f, false);
            yield return new WaitForSeconds(0.08f);


            if (abil.weakened)
            {
                yield return new WaitForSeconds(0.25f);
                player_piece_sprite.CrossFadeColor(Color.red, 0.25f, false, false);
                yield return new WaitForSeconds(0.25f);
                player_piece_sprite.CrossFadeColor(Color.white, 0.25f, false, false);
                yield return new WaitForSeconds(0.25f);

                t_text = player_piece.name.ToUpper() + "'s defences fell!";

                for (int i = 0; i <= t_text.Length; i++)
                {
                    attacking_text.text = t_text.Substring(0, i);
                    attacking_text_shade.text = attacking_text.text;
                    yield return new WaitForSeconds(0.05f);
                }
                yield return new WaitForSeconds(0.5f);

                player_weak = true;
            }

            StartCoroutine(DealDamageToEnemy(n));
            yield return new WaitForSeconds(0.5f);
        }
        if (abil.defense || abil.defensive_strike)
        {

            yield return new WaitForSeconds(0.25f);
            player_piece_sprite.CrossFadeColor(Color.green, 0.25f, false, false);
            yield return new WaitForSeconds(0.25f);
            player_piece_sprite.CrossFadeColor(Color.white, 0.25f, false, false);
            yield return new WaitForSeconds(0.25f);

            t_text = player_piece.name.ToUpper() + "'s defences rose!";
            attacking_menu.SetActive(true);

            for (int i = 0; i <= t_text.Length; i++)
            {
                attacking_text.text = t_text.Substring(0, i);
                attacking_text_shade.text = attacking_text.text;
                yield return new WaitForSeconds(0.05f);
            }

            player_defend = true;
        }

        yield return new WaitForSeconds(0.5f);

        if (abil.defense)
            StartCoroutine(EnemyAnim());
    }

    public IEnumerator DealDamageToEnemy(int n)
    {
        int dmg = Random.Range(player_piece.abilities[n].ability_min, player_piece.abilities[n].ability_max);
        
        if (enemy_defend)
        {
            dmg -= Random.Range(enemy_next_turn.ability_min, enemy_next_turn.ability_max);

            if (dmg < 0)
                dmg = 0;
        }
        else if (enemy_weak)
        {
            dmg = (int)(dmg * 2f);
        }

        if (dmg == 0)
        {
            string t_text = player_piece.name.ToString().ToUpper() + "'s " + player_piece.abilities[n].ability_name.ToString().ToUpper() + " did no damage!";

            attacking_menu.SetActive(true);

            for (int i = 0; i <= t_text.Length; i++)
            {
                attacking_text.text = t_text.Substring(0, i);
                attacking_text_shade.text = attacking_text.text;
                yield return new WaitForSeconds(0.05f);
            }

            yield return new WaitForSeconds(0.5f);
        }

        for (float i = 0; i < dmg; i += 0.5f)
        {
            enemy_health -= 0.5f;
            enemy_health_sprite.fillAmount = enemy_health / enemy_max_health;
            yield return new WaitForSeconds(0.01f);
        }

        if (enemy_health <= 0)
        {
            StartCoroutine(GameEnd(PieceTeam.White));
            yield break;
        }

        if (player_piece.abilities[n].defensive_strike)
        {
            yield return new WaitForSeconds(2.5f);
            StartCoroutine(EnemyAnim());
        }
        else
            StartCoroutine(EnemyAnim());
    }

    public IEnumerator EnemyAnim()
    {
        enemy_weak = false;
        enemy_defend = false;

        string t_text = "Foe " + enemy_piece.name.ToUpper() + " used " + enemy_next_turn.ability_name.ToUpper() + "!";
        attacking_menu.SetActive(true);

        for (int i = 0; i <= t_text.Length; i++)
        {
            attacking_text.text = t_text.Substring(0, i);
            attacking_text_shade.text = attacking_text.text;
            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitForSeconds(0.5f);

        if (!enemy_next_turn.defense)
        {
            for (int i = 0; i < 30; i++)
            {
                enemy_piece_sprite.rectTransform.localPosition = new Vector3(i, enemy_piece_sprite.rectTransform.localPosition.y, 0);
                yield return new WaitForSeconds(Time.deltaTime);
            }
            yield return new WaitForSeconds(0.1f);
            enemy_piece_sprite.rectTransform.localPosition = new Vector3(-5, enemy_piece_sprite.rectTransform.localPosition.y, 0);
            yield return new WaitForSeconds(0.1f);
            enemy_piece_sprite.rectTransform.localPosition = new Vector3(0, enemy_piece_sprite.rectTransform.localPosition.y, 0);

            yield return new WaitForSeconds(0.3f);
            player_piece_sprite.CrossFadeAlpha(0f, 0f, false);
            yield return new WaitForSeconds(0.08f);
            player_piece_sprite.CrossFadeAlpha(1f, 0f, false);
            yield return new WaitForSeconds(0.08f);
            player_piece_sprite.CrossFadeAlpha(0f, 0f, false);
            yield return new WaitForSeconds(0.08f);
            player_piece_sprite.CrossFadeAlpha(1f, 0f, false);
            yield return new WaitForSeconds(0.08f);
            player_piece_sprite.CrossFadeAlpha(0f, 0f, false);
            yield return new WaitForSeconds(0.08f);
            player_piece_sprite.CrossFadeAlpha(1f, 0f, false);

            yield return new WaitForSeconds(0.08f);


            if (enemy_next_turn.weakened)
            {
                yield return new WaitForSeconds(0.25f);
                enemy_piece_sprite.CrossFadeColor(Color.red, 0.25f, false, false);
                yield return new WaitForSeconds(0.25f);
                enemy_piece_sprite.CrossFadeColor(Color.white, 0.25f, false, false);
                yield return new WaitForSeconds(0.25f);

                t_text = "Foe " + enemy_piece.name.ToUpper() + "'s defences fell!";

                for (int i = 0; i <= t_text.Length; i++)
                {
                    attacking_text.text = t_text.Substring(0, i);
                    attacking_text_shade.text = attacking_text.text;
                    yield return new WaitForSeconds(0.05f);
                }
                yield return new WaitForSeconds(0.5f);

                enemy_weak = true;
            }

            StartCoroutine(DealDamageToPlayer());
            yield return new WaitForSeconds(0.5f);
        }

        if (enemy_next_turn.defense || enemy_next_turn.defensive_strike)
        {

            yield return new WaitForSeconds(0.25f);
            enemy_piece_sprite.CrossFadeColor(Color.green, 0.25f, false, false);
            yield return new WaitForSeconds(0.25f);
            enemy_piece_sprite.CrossFadeColor(Color.white, 0.25f, false, false);
            yield return new WaitForSeconds(0.25f);

            t_text = "Foe " + enemy_piece.name.ToUpper() + "'s defences rose!";
            attacking_menu.SetActive(true);

            for (int i = 0; i <= t_text.Length; i++)
            {
                attacking_text.text = t_text.Substring(0, i);
                attacking_text_shade.text = attacking_text.text;
                yield return new WaitForSeconds(0.05f);
            }

            enemy_defend = true;

            yield return new WaitForSeconds(0.5f);
        }

        if (enemy_next_turn.defense)
            attacking_menu.SetActive(false);

    }

    public IEnumerator DealDamageToPlayer()
    {
        int dmg = Random.Range(enemy_next_turn.ability_min, enemy_next_turn.ability_max);

        if (player_defend)
        {
            dmg -= Random.Range(abil.ability_min, abil.ability_max);

            if (dmg < 0)
                dmg = 0;
        }
        else if (player_weak)
        {
            dmg = (int)(dmg * 2f);
        }

        if (dmg == 0)
        {
            string t_text = "Foe's " + enemy_next_turn.ability_name.ToString().ToUpper() + " did no damage!";

            attacking_menu.SetActive(true);

            for (int i = 0; i <= t_text.Length; i++)
            {
                attacking_text.text = t_text.Substring(0, i);
                attacking_text_shade.text = attacking_text.text;
                yield return new WaitForSeconds(0.05f);
            }

            yield return new WaitForSeconds(0.5f);
        }

        for (float i = 0; i < dmg; i += 0.5f)
        {
            player_health -= 0.5f;
            player_health_sprite.fillAmount = player_health / player_max_health;
            player_health_text.text = "" + (int)(player_health) + " / " + player_max_health;
            yield return new WaitForSeconds(0.01f);
        }

        if (player_health <= 0)
        {
            StartCoroutine(GameEnd(PieceTeam.Black));
            yield break;
        }

        if (enemy_next_turn.defensive_strike)
        {
            yield return new WaitForSeconds(3f);
            attacking_menu.SetActive(false);
        }
        else
            attacking_menu.SetActive(false);

        SetEnemyAbility();
    }

    void SetEnemyAbility()
    {
        int n = Random.Range(0, enemy_piece.abilities.Length);

        while (enemy_piece.abilities[n] == null)
            n = Random.Range(0, enemy_piece.abilities.Length);

        enemy_next_turn = enemy_piece.abilities[n];

        if (!enemy_next_turn.defense)
            enemy_indicator.sprite = attack_indicator;
        else
            enemy_indicator.sprite = def_indicator;
    }

    public IEnumerator GameEnd(PieceTeam winner)
    {
        string t_text = player_piece.name.ToUpper() + " fell!";

        if (winner == PieceTeam.White)
            t_text = "Foe's " + enemy_piece.name.ToUpper() + " fell!";

        attacking_menu.SetActive(true);

        for (int i = 0; i <= t_text.Length; i++)
        {
            attacking_text.text = t_text.Substring(0, i);
            attacking_text_shade.text = attacking_text.text;
            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitForSeconds(0.5f);
        flasher.CrossFadeAlpha(1f, 0.25f, false);
        yield return new WaitForSeconds(0.3f);

        ResetUI();
        gm_script.EndSecondary(winner);
        flasher.CrossFadeAlpha(0f, 0f, false);
    }

    public void ResetUI()
    {
        attack_menu.gameObject.SetActive(false);
        attacking_menu.gameObject.SetActive(false);
        combat_menu.gameObject.SetActive(false);
        battle_panel.gameObject.SetActive(false);
        enemy_health_sprite.fillAmount = 1f;
        player_health_sprite.fillAmount = 1f;
    }
}
