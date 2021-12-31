using UnityEngine;

public class Player : MonoBehaviour
{
    public PieceTeam team;

    public Camera cam;
    public Vector2 mouse_pos;
    public GameObject selected_piece;

    public GameManager gm_script;
    public bool can_play;


    void Update()
    {
       // if (!gm_script.MyTurnToPlay(team) || !can_play)
        //    return;

        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (Input.GetMouseButtonDown(0) && hit.collider != null)
        {

            if (hit.collider.tag == gm_script.CUR_TAG)
            {
                // We have a piece selected, and we're moving the piece 
                if (selected_piece != null)
                {
                    gm_script.MovePiece(selected_piece.GetComponent<Piece>(), new Vector2(Mathf.RoundToInt(hit.point.x) - gm_script.current_board.BOARD_OFFSET, Mathf.RoundToInt(hit.point.y)));
                    selected_piece = null;
                    
                }
                // No piece is selected. Select the piece
                // else if (hit.collider.GetComponent<Piece>().team == team)
                else if (hit.collider.GetComponent<Piece>().team == gm_script.current_player)
                {
                    mouse_pos = hit.point;
                    selected_piece = hit.collider.gameObject;
                    gm_script.SelectPiece(hit.collider.GetComponent<Piece>());
                }
            }
            else if (hit.collider.tag == "Square" && selected_piece != null && selected_piece.gameObject.tag == gm_script.CUR_TAG)
            {
                gm_script.MovePiece(selected_piece.GetComponent<Piece>(), new Vector2(Mathf.RoundToInt(hit.point.x) - gm_script.current_board.BOARD_OFFSET, Mathf.RoundToInt(hit.point.y)));
                selected_piece = null;
            }
        }
    }
}
