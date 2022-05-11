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
    public bool cancel_move = false;
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

    public void ChangeBoard()
    {
        cancel_move = true;
        CancelInvoke("MovePiece");
        SetPieces();
    }

    void MovePiece()
    {
        if (cancel_move)
        {
            cancel_move = false;
            return;
        }


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
            gm_script.MovePiece(current_piece, target_square);
        }
        else
        {
            gm_script.MovePiece(current_piece, target_square);
            //Debug.Log(FEN_Notation());
        }
    }
}
