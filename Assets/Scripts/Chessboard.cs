using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chessboard : MonoBehaviour
{
    
    public Square[] squares;
    public Piece[,] board_status = new Piece[9,9];
    public Piece[] all_pieces;
    public List<Piece> black_pieces = new List<Piece>();
    public List<Piece> white_pieces = new List<Piece>();

    public GameObject move_circle;
    public GameObject attack_circle;
    public GameManager gm_script;
    public Sprite white_queen, black_queen, white_pawn, black_pawn;
    public const int BOARD_SIZE = 7;
    public int BOARD_OFFSET = 0;
    public Piece king_attacker;
    public Piece white_king, black_king;

    // Used to reset the board to the original state
    public void ResetPieces()
    {
        Debug.Log("Resetting All Pieces");
        foreach (Piece p in all_pieces)
        {
            if (p == null)
                continue;

            if (!p.gameObject.activeSelf)
                p.gameObject.SetActive(true);

            p.ResetPiece();
        }
        
        RefreshMoves();
    }

    // Used to set the board to its initial state.
    public void SetPieces(string tag)
    {
        Piece p;
        foreach (GameObject g in GameObject.FindGameObjectsWithTag(tag))
        {
            p = g.GetComponent<Piece>();
            p.Init();
            p.guards.Clear();
            board_status[p.position_x, p.position_y] = p;

            if (p.team == PieceTeam.White)
                white_pieces.Add(p);
            else
                black_pieces.Add(p);

            if (p.type == PieceType.King && p.team == PieceTeam.White)
                white_king = p;
            if (p.type == PieceType.King && p.team == PieceTeam.Black)
                black_king = p;
        }
    }
    // Collects the locations of all pieces currently active in the game
    public void RefreshBoard()
    {
        Piece p; 
        black_pieces.Clear();
        white_pieces.Clear();
        // Clear existing list
        for (int i = 0; i < board_status.GetLength(0); i++)
            for (int j = 0; j < board_status.GetLength(1); j++)
                board_status[i, j] = null;

        // Fill with existing piece locations.
        foreach (GameObject g in GameObject.FindGameObjectsWithTag(gm_script.CUR_TAG))
        {
            p = g.GetComponent<Piece>();
            
            board_status[p.position_x, p.position_y] = p;
            if (p.team == PieceTeam.White)
                white_pieces.Add(p);
            else
                black_pieces.Add(p);

            if (p.type == PieceType.King && p.team == PieceTeam.White)
                white_king = p;
            if (p.type == PieceType.King && p.team == PieceTeam.Black)
                black_king = p;
        }

        RefreshMoves();
    }

    // Recalculates all possible moves for all active pieces
    // First recalculate all moves for the opponent, then for active player
    public void RefreshMoves()
    {
        // Go through all pieces and clear pins, targets and guards
        foreach (Piece p in all_pieces)
        {
            p.guards.Clear();
            p.pinned_by = null;
            p.have_target = null;
        }

        if (gm_script.current_player == PieceTeam.White)
        {
            foreach (Piece p in black_pieces)
                p.CalculateMoves();

            foreach (Piece p in white_pieces)
                p.CalculateMoves();
        }
        else if (gm_script.current_player == PieceTeam.Black)
        {
            foreach (Piece p in white_pieces)
                p.CalculateMoves();

            foreach (Piece p in black_pieces)
                p.CalculateMoves();
        }

        bool any_move = false;

        // Check draw
        if (gm_script.current_player == PieceTeam.White)
        {
            foreach (Piece p in white_pieces)
            {
                if (p.valid_moves.Count > 0)
                {
                    any_move = true;
                }
            }

            if (king_attacker != null && !any_move)
            {
                // We have checkmate
                gm_script.Checkmate(PieceTeam.Black);
            }
            else if (king_attacker == null && !any_move)
            {
                // Draw
                gm_script.Checkmate(PieceTeam.None);
            }
        }
        else if (gm_script.current_player == PieceTeam.Black)
        {
            foreach (Piece p in black_pieces)
            {
                if (p.valid_moves.Count > 0)
                {
                    any_move = true;
                }
            }

            if (king_attacker != null && !any_move)
            {
                // We have checkmate
                gm_script.Checkmate(PieceTeam.White);
            }
            else if (king_attacker == null && !any_move)
            {
                // Draw
                gm_script.Checkmate(PieceTeam.None);
            }
        }
    }

    // Is there a piece on the selected square
    public bool IsSquareOccupied(int x, int y)
    {

        if (x < 1 || x > 8 || y < 1 || y > 8)
            return true;

        if (board_status[x, y] == null)
            return false;
        else
            return true;
    }

    // Is the piece on the selected square an enemy
    public bool IsSquareEnemy(PieceTeam t, int x, int y)
    {
        if (x < 1 || x > 8 || y < 1 || y > 8)
            return false;

        if (board_status[x, y] == null)
            return false;

        if (board_status[x, y].team != t)
            return true;
        else
            return false;
    }

    // Is the piece on the selected square a friend
    public bool IsSquareFriend(Piece p, PieceTeam t, int x, int y)
    {
        if (x < 1 || x > 8 || y < 1 || y > 8)
            return false;

        if (board_status[x, y] == null)
            return false;

        if (board_status[x, y].team != t)
            return false;

        if (p.type == PieceType.Pawn)
            return false;

        board_status[x, y].guards.Add(p);

        return true;
        
    }

    public bool IsSquareEnemyKing(PieceTeam t, int x, int y)
    {
        if (x < 1 || x > 8 || y < 1 || y > 8)
            return false;

        if (board_status[x, y] == null)
            return false;

        if (board_status[x, y].team != t && board_status[x, y].type == PieceType.King)
            return true;
        else
            return false;
    }

    public bool IsGuarded(int x, int y)
    {
        if (board_status[x, y].guards.Count > 0)
            return true;
        else
            return false;
    }    

    // TODO: Rework
 
    // Checks if that square can be moved to by a piece of a certain team. Used to determine if king is in checkmate.
    public bool IsSquareMovableTo(PieceTeam t, int x, int y)
    {
        if (t == PieceTeam.Black)
        {
            foreach (Piece p in white_pieces)
            {
                if (p.valid_moves.Contains(new Vector2(x, y))
                    || p.attack_moves.Contains(new Vector2(x, y)))
                        return false;
            }
        }
        else if (t == PieceTeam.White)
        {
            foreach (Piece p in black_pieces)
            {
                if (p.valid_moves.Contains(new Vector2(x, y))
                    || p.attack_moves.Contains(new Vector2(x, y)))
                        return false;
            }
        }

        return true;
    }

    public Piece MyKing(PieceTeam t)
    {
        if (t == PieceTeam.White)
            return white_king;
        else if (t == PieceTeam.Black)
            return black_king;
        else
            return null;
    }

    // Check if this move will result in the king being checked.
    public bool IsLineOfFire(PieceTeam t, int x, int y)
    {
        if (t == PieceTeam.Black)
        {
            foreach (Piece p in white_pieces)
            {

                if (p.type == PieceType.Pawn)
                {
                    if (p.position_y + 1 == y
                        && (p.position_x + 1 == x || p.position_x - 1 == x))
                        return true;
                }
                else if (p.valid_moves.Contains(new Vector2(x, y))
                    || p.attack_moves.Contains(new Vector2(x, y)))
                    return true;
            }
        }
        else if (t == PieceTeam.White)
        {
            foreach (Piece p in black_pieces)
            {

                if (p.type == PieceType.Pawn)
                {
                    if (p.position_y - 1 == y
                        && (p.position_x + 1 == x || p.position_x - 1 == x))
                        return true;
                }
                else if (p.valid_moves.Contains(new Vector2(x, y))
                    || p.attack_moves.Contains(new Vector2(x, y)))
                    return true;
            }
        }

        return false;
    }

    public bool IsKingInCheck(PieceTeam t)
    {
        Piece king_;

        if (t == PieceTeam.White)
            king_ = white_king;
        else
            king_ = black_king;

        // Check enemy attack moves to see if anyone is targeting the king
        for (int i = 0; i < board_status.GetLength(0); i++)
        {
            for (int j = 0; j < board_status.GetLength(1); j++)
            {
                if (board_status[i, j] == null)
                    continue;

                if (board_status[i, j].team != t && 
                    board_status[i, j].attack_moves.Contains(new Vector2(king_.position_x, king_.position_y)))
                {
                    king_attacker = board_status[i, j];
                    return true;
                }
                    
            }
        }

        return false;
    }

    public void ShowPossibleMoves(Piece piece)
    {
        HideMoves();

        foreach (Vector2 pos in piece.valid_moves)
        {
            Instantiate(move_circle, new Vector2(pos.x + BOARD_OFFSET, pos.y), Quaternion.identity);
        }

        foreach (Vector2 pos in piece.attack_moves)
        {
            Instantiate(attack_circle, new Vector2(pos.x + BOARD_OFFSET, pos.y), Quaternion.identity);
        }
    }

    // Different int values mean different results
    // 0 = No move
    // 1 = Move to empty square
    // 2 = Attack piece
    // 3 = En-Passant
    // 5 = Attack King
    public int MoveTo(Piece piece, Vector2 pos)
    {
        HideMoves();
        
        // Ignore move if the position is the same as the piece's
        if (pos == new Vector2(piece.position_x, piece.position_y))
            return 0;

        if (piece.valid_moves.Contains(pos))
        {
            // Remove the piece from its existing slot on the board
            board_status[piece.position_x, piece.position_y] = null;

            // Move and re-add to the board
            piece.MoveTo((int)pos.x, (int)pos.y);
            board_status[(int)pos.x, (int)pos.y] = piece;

            // Tell the Game Manager that a piece has been moved.
            return 1;
        }
        else if (piece.attack_moves.Contains(pos))
        {
            // Pawn En Passant
            if (piece.team == PieceTeam.White && piece.type == PieceType.Pawn
                && board_status[(int)pos.x, (int)pos.y] == null
                && board_status[(int)pos.x, (int)pos.y - 1] != null)
            {
                board_status[(int)pos.x, (int)pos.y - 1].gameObject.SetActive(false);
                piece.MoveTo((int)pos.x, (int)pos.y);
                return 3;
            }
                
            // En Passant for Black
            else if (piece.team == PieceTeam.Black && piece.type == PieceType.Pawn
                && board_status[(int)pos.x, (int)pos.y] == null
                && board_status[(int)pos.x, (int)pos.y + 1] != null)
            {
                board_status[(int)pos.x, (int)pos.y + 1].gameObject.SetActive(false);
                piece.MoveTo((int)pos.x, (int)pos.y);
                return 3;
            }

            // All other attacks
            else  if (board_status[(int)pos.x, (int)pos.y].type == PieceType.King && !gm_script.playing_main)
            {
                return 4;
            }
            // All regular attacks otherwise, de-activate victim and move attacker to victim's place
            else if (!gm_script.playing_main)
            {
                board_status[(int)pos.x, (int)pos.y].gameObject.SetActive(false);
                piece.MoveTo((int)pos.x, (int)pos.y);
                return 2;
            }
            else
            {
                return 2;
            }
        }

        return 0;
    }
 
    public void HideMoves()
    {
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Circle"))
        {
            Destroy(g);
        }
    }
}
