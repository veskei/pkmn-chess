using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum GameState
{
    None = 0,
    Initialized = 1,
    WhiteTurn = 2,
    BlackTurn = 3,
    WhiteInCheck = 4, // White's turn but they are in check
    BlackInCheck = 5, // Black's turn but they are in check
    WhiteWon = 6,
    BlackWon = 7
}
public class GameManager : MonoBehaviour
{
    public PkmChess pkmn_script;
    public Chessboard main_board, secondary_board, current_board;
    public Player player;
    public PieceTeam current_player;
    PieceTeam main_player;
    public GameState state, secondary_state;
    public Text game_state;
    public bool playing_main;
    public GameObject[] cam_positions;
    public GameObject cam;
    public int cur_pos = 0;
    public int game_type = 1;
    public float lerp_speed;
    public Computer comp;
    public string MAIN_TAG, SEC_TAG, CUR_TAG;
    public Piece s_attacker, s_defender; // Pieces that are involved in the fight on the second board. loser dies.

    public GameObject end_screen;
    public Text end_text;
    // For fight animation
    public Image top, bottom;
    Vector3 start_top, start_bottom;
    // Start is called before the first frame update
    void Start()
    {
        try
        {
            game_type = GameObject.FindGameObjectWithTag("GameMode").GetComponent<GameMode>().game_type;
            GameObject.FindGameObjectWithTag("GameMode").GetComponent<GameMode>().gm_script = this;
        }
        catch
        {
            Debug.Log("Game Type not found. Using Default");
        }
        
        current_board = main_board;

        top.CrossFadeAlpha(0, 0.0f, false);
        bottom.CrossFadeAlpha(0, 0.0f, false);

        main_board.SetPieces(MAIN_TAG);
        main_board.RefreshBoard();

        secondary_board.SetPieces(SEC_TAG);

        comp.SetPieces();
        start_top = top.rectTransform.localPosition;
        start_bottom = bottom.rectTransform.localPosition;
        state = GameState.Initialized;
    }

    // Update is called once per frame
    void Update()
    {
        game_state.text = "Status: " + state;
        cam.transform.position = Vector3.Lerp(cam.transform.position, cam_positions[cur_pos].transform.position, Time.deltaTime*lerp_speed);
    }

    public void MovePiece (Piece p, Vector2 pos)
    {
        int state_ = current_board.MoveTo(p, pos);
        Debug.Log("State: " + state_);

        if (game_type == 1)
        {
            if (state_ == 2 && playing_main)
            {
                StartCoroutine(StartSecondary(p, current_board.board_status[(int)pos.x, (int)pos.y]));
                return;
            }

            if (state_ == 4 && playing_main)
            {
                // The king is the victim.
                StartCoroutine(StartSecondary(p, current_board.board_status[(int)pos.x, (int)pos.y]));
                return;
            }
        }
        else if (state_ == 2 && game_type == 2)
        {
            player.can_play = false;
            s_attacker = p;
            s_defender = current_board.board_status[(int)pos.x, (int)pos.y];
            p.transform.position = Vector3.Lerp(p.transform.position, current_board.board_status[(int)pos.x, (int)pos.y].transform.position, 0.95f);
            main_player = (PieceTeam)((int)current_player * (int)PieceTeam.Black);
            pkmn_script.GameStart(s_attacker, s_defender);
            return;
        }

        if (state_ == 1 || state_ == 3 || (state_ == 2 && !playing_main))
        {
            if (current_player == PieceTeam.White)
            {
                comp.cancel_move = false;
                state = GameState.BlackTurn;
                current_player = PieceTeam.Black;
                current_board.RefreshBoard();
                comp.MoveRandomPiece();
            }
            else if (current_player == PieceTeam.Black)
            {
                state = GameState.WhiteTurn;
                current_player = PieceTeam.White;
            }

            current_board.RefreshBoard();
        }
    }

    public void SelectPiece (Piece p)
    {
        current_board.ShowPossibleMoves(p);
    }


    public bool MyTurnToPlay(PieceTeam team)
    {
        if (team == current_player)
            return true;
        else
            return false;
    }

    public IEnumerator StartSecondary(Piece attacker, Piece defender)
    {
        player.can_play = false;
        main_player = (PieceTeam)((int)current_player * (int)PieceTeam.Black);
        s_attacker = attacker;
        s_defender = defender;
        CUR_TAG = SEC_TAG;
        current_board = secondary_board;
        current_board.ResetPieces();
        attacker.transform.position = Vector3.Lerp(attacker.transform.position, defender.transform.position, 0.95f);
        

        // Play fight animation
        top.CrossFadeAlpha(1, 0.4f, false);
        bottom.CrossFadeAlpha(1, 0.4f, false);
        yield return new WaitForSeconds(0.5f);

        for (float i = 1; i < 100; i *= 1.33f) 
        {
            if (i > 100)
                i = 100;

            top.rectTransform.localPosition = Vector3.Lerp(top.rectTransform.localPosition, new Vector3(0, 2, 0), i / 100);
            bottom.rectTransform.localPosition = Vector3.Lerp(bottom.rectTransform.localPosition, new Vector3(0, -1, 0), i / 100);

            yield return new WaitForSeconds(0.01f);

        }

        yield return new WaitForSeconds(0.5f);
        top.CrossFadeAlpha(0, 0.1f, false);
        bottom.CrossFadeAlpha(0, 0.1f, false);

        cur_pos = 1;

        yield return new WaitForSeconds(0.1f);

        playing_main = false;
        current_player = PieceTeam.White;
        current_board.RefreshBoard();
        comp.ChangeBoard();
        top.rectTransform.localPosition = start_top;
        bottom.rectTransform.localPosition = start_bottom;
        player.can_play = true;
    }

    public void GameWon(PieceTeam winning_team)
    {
        if (playing_main)
        {
            SceneManager.LoadScene(0);
            return;
        }

        if (!playing_main)
            EndSecondary(winning_team);

    }

    public void EndSecondary(PieceTeam winner)
    {
        Debug.Log("Ending Secondary");
        current_board = main_board;
        comp.cancel_move = true;

        if (winner == s_attacker.team)
        {
            Debug.Log("aa");
            s_attacker.MoveTo(s_defender.position_x, s_defender.position_y);
            s_defender.gameObject.SetActive(false);

            if (s_defender.type == PieceType.King)
                StartCoroutine(GameEnd(s_attacker.team));
            Debug.Log("ab");
        }
        else if (winner == s_defender.team)
        {
            Debug.Log("ac");
            s_attacker.gameObject.SetActive(false);

            if (s_attacker.type == PieceType.King)
                StartCoroutine(GameEnd(s_defender.team));
            Debug.Log("ad");
        }

        Debug.Log("ae");

        current_player = PieceTeam.White;
        state = GameState.WhiteTurn;
        playing_main = true;
        cur_pos = 0;
        CUR_TAG = MAIN_TAG;
        current_board.RefreshBoard();

        Debug.Log("af");

        if (game_type == 1)
            Invoke("ResetSecondary", 1f);

        player.can_play = true;

        Debug.Log("ag");
    }

    public void ResetSecondary()
    {
        Debug.Log("Resetting Secondary Board");
        secondary_board.ResetPieces();
    }

    public void Checkmate(PieceTeam winner)
    {
        if (playing_main)
            return;

        EndSecondary(winner);
    }

    public IEnumerator GameEnd(PieceTeam winner)
    {
        // Show End screen.
        player.enabled = false;
        yield return new WaitForSeconds(1f);
        Time.timeScale = 0f;

        if (winner == player.team)
            end_text.text = "You Win!";
        else
            end_text.text = "Womp womp. \nYou Lose!";

        end_screen.SetActive(true);
    }

    public void RestartGame()
    {
        // Give option to restart game mode
        Time.timeScale = 1f;
        GameObject.FindGameObjectWithTag("GameMode").GetComponent<GameMode>().RestartGame();
    }

    public void BackToMenu()
    {
        // Give option to go back to main menu.
        Time.timeScale = 1f;
        GameObject.FindGameObjectWithTag("GameMode").GetComponent<GameMode>().BackToMenu();
    }

    public void Concede()
    {
        // Give option to go back to main menu.
        StartCoroutine(GameEnd(PieceTeam.Black));
    }
}
