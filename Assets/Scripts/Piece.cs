using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PieceType
{
    None = 0,
    Pawn = 1,
    Rook = 2,
    Knight = 3,
    Bishop = 4,
    Queen = 5,
    King = 6
}

public enum PieceTeam
{
    None = 0,
    Black = -1,
    White = 1
}

public class Piece : MonoBehaviour
{
    public PieceTeam team;
    public PieceType type;

    public List<Vector2> valid_moves, attack_moves;
    public List<Piece> guards;
    public Chessboard board;
    
    public int moves = 0;
    public Vector3 start_pos;

    // True relative to board (1 to 8 for x and y)
    public int position_x; 
    public int position_y;
    
    public Piece pinned_by = null;

    // used to calculate is pinned by looking 'through target' to determine if king is behind.
    // have_target will be the piece pinned if the king is indeed behind.
    public Piece have_target = null; 
    public bool castle_short = false, castle_long = false;
    public bool en_passant = false;

    public void Init()
    {
        position_x = Mathf.RoundToInt(transform.position.x) - board.BOARD_OFFSET;
        position_y = Mathf.RoundToInt(transform.position.y);
        start_pos = this.transform.position;
    }

    public void MoveTo(int x, int y)
    {
        if (type == PieceType.King && moves == 0)
        {
            // Check if castling short.
            if (x == 7)
            { 
                board.board_status[8, position_y].MoveTo(6, position_y);
                castle_long = false;
                castle_short = false;
            }
            // Check if castling long
            else if (x == 3)
            {
                board.board_status[1, position_y].MoveTo(4, position_y);
                castle_long = false;
                castle_short = false;
            }
        }

        transform.position = new Vector3(x+ board.BOARD_OFFSET, y, transform.position.z);
        position_x = x;
        position_y = y;
        moves++;
    }

    public void CalculateMoves()
    {
        valid_moves.Clear();
        attack_moves.Clear();
        int dir_x = 0;
        int dir_y = 0;
        int pawn_dir = 1;
        Vector2 new_pos;
        
        if (type == PieceType.Pawn)
        {
            if (team == PieceTeam.Black)
                pawn_dir = -1;

            for (int x = -1; x <= 1; x++)
            {
                for (int y = 1; y <= 2; y++)
                {
                    // Making sure the Pawn can only move two spots if its the first move.
                    if (moves > 0 && y == 2)
                        break;

                    dir_x = position_x + x;
                    dir_y = position_y + y * pawn_dir;
                    new_pos = new Vector2(dir_x, dir_y);

                    if (board.IsSquareEnemy(team, dir_x, dir_y) && x != 0)
                    {
                        attack_moves.Add(new_pos);
                        break;
                    }
                    else if (board.IsSquareFriend(this, team, dir_x, dir_y))
                    {
                        break;
                    }
                    else if (!board.IsSquareOccupied(dir_x, dir_y) && x == 0)
                    {
                        valid_moves.Add(new_pos);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            PawnSpecialRules();
        }
       
        if (type == PieceType.King)
        {
            // TODO: Prevent King from walking into line of fire
            // TODO: Prevent King from attacking piece that is protected
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    dir_x = position_x + x;
                    dir_y = position_y + y;

                    new_pos = new Vector2(dir_x, dir_y);

                    if (board.IsLineOfFire(team, dir_x, dir_y))
                        Debug.Log(team + ". Line of fire. King Cannot move there.");
                    else if (board.IsSquareEnemy(team, dir_x, dir_y) && !board.IsGuarded(dir_x, dir_y))
                        attack_moves.Add(new_pos);
                    else if (board.IsSquareFriend(this, team, dir_x, dir_y))
                        continue;
                    else if (!board.IsSquareOccupied(dir_x, dir_y))
                        valid_moves.Add(new_pos);

                }
            }

            if (moves == 0)
                KingSpecialRules();
        }

        if (type == PieceType.Knight)
        {
            // Special movement for the knight
            for (int x = -2; x <= 2; x++)
            {
                for (int y = -1; y <= 1; y += 2)
                {
                    if (x == 0 || y == 0)
                        continue;

                    dir_x = position_x + x;

                    if (Mathf.Abs(x) == 2)
                        dir_y = position_y + y;
                    else
                        dir_y = position_y + (y * 2);

                    new_pos = new Vector2(dir_x, dir_y);

                    if (board.IsSquareEnemy(team, dir_x, dir_y))
                        attack_moves.Add(new_pos);
                    else if (board.IsSquareFriend(this, team, dir_x, dir_y))
                        continue;
                    else if (!board.IsSquareOccupied(dir_x, dir_y))
                        valid_moves.Add(new_pos);
                }
            }
        }

        if (type == PieceType.Rook || type == PieceType.Queen)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    have_target = null;

                    // Ignore self
                    if (x == 0 && y == 0)
                        continue;

                    // No diagonals
                    if (x != 0 && y != 0)
                        continue;

                    for (int step = 1; step <= Chessboard.BOARD_SIZE; step++)
                    {
                        dir_x = position_x + (step * x);
                        dir_y = position_y + (step * y);
                        new_pos = new Vector2(dir_x, dir_y);

                        if (board.IsSquareEnemy(team, dir_x, dir_y))
                        {
                            if (have_target != null && board.board_status[dir_x, dir_y].type == PieceType.King)
                            {
                                have_target.pinned_by = this;
                                break;
                            }
                            else if (have_target == null)
                            {
                                attack_moves.Add(new_pos);
                                have_target = board.board_status[dir_x, dir_y];
                            }
                            else if (have_target != null)
                            {
                                break;
                            }
                        }
                        else if (have_target == null)
                        {
                            if (board.IsSquareFriend(this, team, dir_x, dir_y))
                            {
                                break;
                            }
                            else if (!board.IsSquareOccupied(dir_x, dir_y) && have_target == null)
                            {
                                valid_moves.Add(new_pos);
                            }
                            else
                            {
                                break;
                            }
                        }                        
                    }
                }
            }
        }

        if (type == PieceType.Bishop || type == PieceType.Queen)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    have_target = null;

                    // Ignore self
                    if (x == 0 && y == 0)
                        continue;

                    // No straight lines
                    if (x == 0 || y == 0)
                        continue;

                    for (int step = 1; step <= Chessboard.BOARD_SIZE; step++)
                    {
                        dir_x = position_x + (step * x);
                        dir_y = position_y + (step * y);
                        new_pos = new Vector2(dir_x, dir_y);

                        if (board.IsSquareEnemy(team, dir_x, dir_y))
                        {
                            if (have_target != null && board.board_status[dir_x, dir_y].type == PieceType.King)
                            {
                                have_target.pinned_by = this;
                                break;
                            }
                            else if (have_target == null)
                            {
                                attack_moves.Add(new_pos);
                                have_target = board.board_status[dir_x, dir_y];
                            }
                            else if (have_target != null)
                            {
                                break;
                            }
                        }
                        else if (board.IsSquareFriend(this, team, dir_x, dir_y))
                        {
                            break;
                        }
                        else if (!board.IsSquareOccupied(dir_x, dir_y) && have_target == null)
                        {
                            valid_moves.Add(new_pos);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }

        if (board.gm_script.playing_main)
            return;

        // When the king is checked, only allow moves that block the check or attack the piece targeting the king.
        if (board.IsKingInCheck(team) && type != PieceType.King)
        {
            List<Vector2> new_moves = new List<Vector2>();
            List<Vector2> new_attacks = new List<Vector2>();

            int king_pos_x = board.MyKing(team).position_x;
            int king_pos_y = board.MyKing(team).position_y;
            int att_pos_x = board.king_attacker.position_x;
            int att_pos_y = board.king_attacker.position_y;

            // If king and attacker are on the same row
            if (king_pos_y - att_pos_y == 0)
            {
                for (int x = Mathf.Min(att_pos_x, king_pos_x); x <= Mathf.Max(att_pos_x, king_pos_x); x++)
                {
                    if (board.king_attacker.valid_moves.Contains(new Vector2(x, king_pos_y))
                         && valid_moves.Contains(new Vector2(x, king_pos_y)))
                        new_moves.Add(new Vector2(x, king_pos_y));
                }
            }
            // If king and attacker are on same column
            else if (king_pos_x - att_pos_x == 0)
            {
                for (int y = Mathf.Min(att_pos_y, king_pos_y); y <= Mathf.Max(att_pos_y, king_pos_y); y++)
                {
                    if (board.king_attacker.valid_moves.Contains(new Vector2(king_pos_x, y))
                        && valid_moves.Contains(new Vector2(king_pos_x, y)))
                        new_moves.Add(new Vector2(king_pos_x, y));
                }
            }
            // If pieces are diagonal
            else
            {
                for (int x = 0; x <= Mathf.Max(att_pos_x, king_pos_x) - Mathf.Min(att_pos_x, king_pos_x); x++) 
                {
                    for (int y = 0; y <= Mathf.Max(att_pos_y, king_pos_y) - Mathf.Min(att_pos_y, king_pos_y); y++) 
                    {
                        if (x != y)
                            continue;

                        // If attacker is further right K - - A
                        if (att_pos_x - king_pos_x > 0) 
                        {
                            if (att_pos_y - king_pos_y > 0)
                            {
                                dir_x = king_pos_x + x;
                                dir_y = king_pos_y + y;
                            }
                            else if (att_pos_y - king_pos_y < 0) // TBD
                            {
                                dir_x = king_pos_x + x;
                                dir_y = king_pos_y - y;
                            }
                            
                        }
                        else if (att_pos_x - king_pos_x < 0)
                        {
                            if (att_pos_y - king_pos_y > 0)
                            {
                                dir_x = king_pos_x - x;
                                dir_y = king_pos_y + y;
                            }
                            else if (att_pos_y - king_pos_y < 0)
                            {
                                dir_x = king_pos_x - x;
                                dir_y = king_pos_y - y;
                            }
                        }

                        if (board.king_attacker.valid_moves.Contains(new Vector2(dir_x, dir_y))
                            && valid_moves.Contains(new Vector2(dir_x, dir_y)))
                            new_moves.Add(new Vector2(dir_x, dir_y));
                    }
                }
            }
                        
            // Check if piece can attack the attacker
            foreach (Vector2 v in attack_moves)
            {
                if (v == new Vector2(board.king_attacker.position_x, board.king_attacker.position_y))
                {
                    new_attacks.Add(v);
                    break;
                }
            }

            valid_moves.Clear();
            valid_moves = new_moves;

            attack_moves.Clear();
            attack_moves = new_attacks;
        } 
        
        // Is this piece pinned due to being in line with the king?
        if (pinned_by != null && type != PieceType.King) 
        {
            List<Vector2> valid_attacks = new List<Vector2>();
            List<Vector2> new_moves = new List<Vector2>();

            foreach (Vector2 v in valid_moves)
            {
                if ((v.x == pinned_by.position_x && v.x == board.MyKing(team).position_x) 
                    || (v.y == pinned_by.position_y && v.y == board.MyKing(team).position_y))
                {
                    new_moves.Add(v);
                }
            }
            
            foreach (Vector2 v in attack_moves)
            {
                if (v == new Vector2 (pinned_by.position_x, pinned_by.position_y))
                {
                    valid_attacks.Add(v);
                    break;
                }
            }


            pinned_by = null;

            valid_moves.Clear();
            valid_moves = new_moves;
            attack_moves.Clear();
            attack_moves = valid_attacks;
        }
    }

    public void PawnSpecialRules()
    {
        en_passant = false;
        if (team == PieceTeam.White)
        {
            // En Passant for White
            if (position_y == 5)
            {
                // En passant left
                if (board.IsSquareEnemy(team, position_x - 1, position_y)
                    && board.board_status[position_x - 1, position_y].moves == 1 
                    && board.board_status[position_x - 1, position_y].type == PieceType.Pawn)
                { 
                    attack_moves.Add(new Vector2(position_x - 1, position_y + 1));
                    en_passant = true;
                    board.board_status[position_x - 1, position_y].moves++;
                }
                // En passant right.
                if (board.IsSquareEnemy(team, position_x + 1, position_y)
                    && board.board_status[position_x + 1, position_y].moves == 1
                    && board.board_status[position_x + 1, position_y].type == PieceType.Pawn)
                {
                    attack_moves.Add(new Vector2(position_x + 1, position_y + 1));
                    en_passant = true;
                    board.board_status[position_x + 1, position_y].moves++;
                }
        }
            // Promotion for White
            if (position_y == 8)
            {
                type = PieceType.Queen;
                this.GetComponent<SpriteRenderer>().sprite = board.white_queen;
            }
        }
        else if (team == PieceTeam.Black)
        {
            // En Passant for Black
            if (position_y == 4)
            {
                // En passant left
                if (board.IsSquareEnemy(team, position_x - 1, position_y)
                    && board.board_status[position_x - 1, position_y].moves == 1
                    && board.board_status[position_x - 1, position_y].type == PieceType.Pawn)
                { 
                    attack_moves.Add(new Vector2(position_x - 1, position_y - 1));
                    en_passant = true;
                    board.board_status[position_x - 1, position_y].moves++;
                }
                // En passant right.
                if (board.IsSquareEnemy(team, position_x + 1, position_y)
                    && board.board_status[position_x + 1, position_y].moves == 1
                    && board.board_status[position_x + 1, position_y].type == PieceType.Pawn)
                {
                    attack_moves.Add(new Vector2(position_x + 1, position_y - 1));
                    en_passant = true;
                    board.board_status[position_x + 1, position_y].moves++;
                }
            }
            // Promotion for Black
            if (position_y == 1)
            {
                type = PieceType.Queen;
                this.GetComponent<SpriteRenderer>().sprite = board.black_queen;
            }
            
        }
    }

    public void KingSpecialRules()
    {
        // Castling King Side
        if (board.board_status[8, position_y] != null)
        {
            if (!board.IsSquareOccupied(6, position_y) 
             && !board.IsSquareOccupied(7, position_y)
             && board.IsSquareMovableTo(team, 6, position_y)
             && board.IsSquareMovableTo(team, 7, position_y)
             && board.king_attacker == null
             && board.board_status[8, position_y].moves == 0 && moves == 0)
            {
                valid_moves.Add(new Vector2(7, position_y));
                castle_short = true;
            }
        }
        else
            castle_short = false;

        // Castling Queen Side
        if (board.board_status[1, position_y] != null)
        {
            if (!board.IsSquareOccupied(2, position_y)
                && !board.IsSquareOccupied(3, position_y)
                && !board.IsSquareOccupied(4, position_y)
                && board.IsSquareMovableTo(team, 2, position_y)
                && board.IsSquareMovableTo(team, 3, position_y)
                && board.IsSquareMovableTo(team, 4, position_y)
                && board.king_attacker == null
                && board.board_status[1, position_y].moves == 0 && moves == 0)
            {
                valid_moves.Add(new Vector2(3, position_y));
                castle_long = true;
            }
        }
        else
            castle_long = false;
    }

    public void ResetPiece()
    {
        if (start_pos.y == 2 && type == PieceType.Queen)
        {
            type = PieceType.Pawn;
            this.GetComponent<SpriteRenderer>().sprite = board.white_pawn;
        }
        else if (start_pos.y == 7 && type == PieceType.Queen)
        {
            type = PieceType.Pawn;
            this.GetComponent<SpriteRenderer>().sprite = board.black_pawn;
        }

        moves = 0;
        this.transform.position = start_pos;
        position_x = Mathf.RoundToInt(transform.position.x - board.BOARD_OFFSET);
        position_y = Mathf.RoundToInt(transform.position.y);
    }
}
