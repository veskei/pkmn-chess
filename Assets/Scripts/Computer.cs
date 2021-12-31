using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum PieceValue
{
    None = -1000,
    Pawn = 10,
    Rook = 50,
    Knight = 30,
    Bishop = 30,
    Queen = 90,
    King = 900
}

public class Computer : MonoBehaviour
{
    public GameManager gm_script;
    public PieceTeam team;
    public List<Piece> my_pieces = new List<Piece>();

    public void SetPieces()
    {
        my_pieces.Clear();

        for (int i = 0; i < gm_script.current_board.board_status.GetLength(0); i++)
        {
            for (int j = 0; j < gm_script.current_board.board_status.GetLength(1); j++)
            {
                if (gm_script.current_board.board_status[i, j] == null)
                    continue;

                if (gm_script.current_board.board_status[i, j].team == team)
                    my_pieces.Add(gm_script.current_board.board_status[i, j]);
            }
        }
    }


    public void MoveRandomPiece()
    {
        Invoke("MovePiece", 0.5f);
    }

    void MovePiece()
    {
        PieceValue current_target = PieceValue.None;
        Piece current_piece = null;
        Vector2 target_square = Vector2.zero;
        PieceType current_type;

        for (int i = 0; i < my_pieces.Count; i++)
        {
            if (!my_pieces[i].gameObject.activeSelf)
            {
                my_pieces.RemoveAt(i);
                continue;
            }

            foreach (Vector2 v in my_pieces[i].attack_moves)
            {
                current_type = gm_script.current_board.board_status[(int)v.x, (int)v.y].type;

                if ((PieceValue)current_type > current_target && Random.Range(0,100) >= 50)
                {
                    current_target = (PieceValue)current_type;
                    current_piece = my_pieces[i];
                    target_square = v;
                }
            }
        }

        if (current_target == PieceValue.None)
        {
            // No attack moves found
            while (current_piece == null)
            {
                current_piece = my_pieces[Random.Range(0, my_pieces.Count)];
                if (current_piece.valid_moves.Count == 0)
                    current_piece = null;

            }
            target_square = current_piece.valid_moves[Random.Range(0, current_piece.valid_moves.Count)];
            gm_script.current_board.MoveTo(current_piece, target_square);
        }
        else
        {
            gm_script.current_board.MoveTo(current_piece, target_square);
            Debug.Log(FEN_Notation());
        }
    }

    string FEN_Notation()
    {
        int empty_ranks = 0;
        string placement = "";
        string castling = "";
        string w_k = "", w_q = "", b_k = "", b_q = "";
        string en_passant = "";
        char cur_piece;
        Piece p;

        for (int y = gm_script.current_board.board_status.GetLength(1)-1; y > 0; y--)
        {
            for (int x = 1; x <= gm_script.current_board.board_status.GetLength(0) - 1; x++) 
            {
                
                if (gm_script.current_board.board_status[x, y] == null && x == gm_script.current_board.board_status.GetLength(0) - 1)
                {
                    empty_ranks++;
                    placement += empty_ranks.ToString();
                    empty_ranks = 0;
                    continue;
                }
                else if (gm_script.current_board.board_status[x, y] == null)
                {
                    empty_ranks++;
                    continue;
                }
                else if (gm_script.current_board.board_status[x, y] != null && empty_ranks > 0)
                {
                    placement += empty_ranks.ToString();
                    empty_ranks = 0;
                }

                p = gm_script.current_board.board_status[x, y];

                if (p.type == PieceType.Pawn)
                {
                    cur_piece = 'p';
                    if (p.en_passant)
                    {
                        en_passant += (Files)p.position_x + "" +  p.position_y;
                    }
                }
                else if (p.type == PieceType.Knight)
                    cur_piece = 'n';
                else if (p.type == PieceType.Bishop)
                    cur_piece = 'b';
                else if (p.type == PieceType.Queen)
                    cur_piece = 'q';
                else if (p.type == PieceType.King)
                {
                    cur_piece = 'k';

                    // Tagging castling moves if they are available
                    if (p.castle_long && p.team == PieceTeam.White)
                        w_q = "Q";
                    if (p.castle_short && p.team == PieceTeam.White)
                        w_k = "K";
                    if (p.castle_long && p.team == PieceTeam.Black)
                        b_q = "q";
                    if (p.castle_short && p.team == PieceTeam.Black)
                        b_k = "k";
                }
                else if (p.type == PieceType.Rook)
                    cur_piece = 'r';
                else
                    cur_piece = 'a';

                if (p.team == PieceTeam.White)
                    placement += cur_piece.ToString().ToUpper();
                else
                    placement += cur_piece.ToString();
            }
            placement += "/";
        }

        
        placement = placement.Substring(0, placement.Length - 1);
        castling = w_k + w_q + b_k + b_q;

        if (castling.Length == 0)
            castling = "-";

        placement = placement + " " + gm_script.current_player.ToString().Substring(0, 1).ToLower() + " " + castling + " 1 2";
        return placement;
    }
}
