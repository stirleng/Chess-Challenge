using ChessChallenge.API;
using System.Security.Cryptography;
using System.Xml.Linq;


public class MyBot : IChessBot
{
    //Constants
    //=========

    //Color (set at game start)
    int am_white;

    //Piece Values (centipawns)
    int[] piece_val_arr = { 100, 300, 300, 500, 900 };
    //=========

    //Search Depth
    int search_depth = 4;
    // \Constants
    public Move Think(Board board, Timer timer)
    {
        am_white = board.IsWhiteToMove ? 1 : -1;
        int best_score = int.MinValue;
        Move[] moves = board.GetLegalMoves();
        Move best_move = moves[0];
        foreach (Move move in moves) 
        {
            board.MakeMove(move);
            int pos_eval = -negamax(board, search_depth-1, am_white*-1);
            if (pos_eval > best_score)
            {
                best_score = pos_eval;
                best_move = move;
            }
            board.UndoMove(move);
        }
        return best_move;
    }

    public int negamax(Board board, int depth, int color) //white is 1, black is -1
    {
        if (depth == 0 || board.IsInCheckmate())
            return color * evaluate(board);
        int score = int.MinValue;
        Move[] moves = board.GetLegalMoves();
        foreach (Move move in moves)
        {
            board.MakeMove(move);
            int search_score = -negamax(board, depth - 1, -1 * color);
            board.UndoMove(move);
            score = (score > search_score) ? score : search_score;
        }
        return score;
    }

    public int evaluate(Board board)
    {
        int white_points = 0;  //material counts (centipawns)
        int black_points = 0;

        PieceList[] piece_lists = board.GetAllPieceLists();     //getting pieces

        for (int i = 0; i < 11; i++) //stop at 10 to skip black king value
        {
            if (i == 5) //skip white king value
                continue;
            PieceList piece_type_list = piece_lists[i];
            int piece_count = piece_type_list.Count;
            if (i < 6) //white pieces
            {
                white_points += piece_count * piece_val_arr[i];
            }
            else //black pieces
            {
                black_points += piece_count * piece_val_arr[i - 6];
            }
        }
        int eval = white_points-black_points;
        return eval;
    }
}