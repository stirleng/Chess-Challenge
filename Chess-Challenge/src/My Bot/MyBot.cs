using ChessChallenge.API;
using System.Security.Cryptography;


public class MyBot : IChessBot
{
    //Constants
    //=========

    //Color (set at game start)
    bool am_white;

    //Piece Values (centipawns)
    int[] piece_val_arr = { 100, 300, 300, 500, 900 };
    //=========

    // \Constants
    public Move Think(Board board, Timer timer)
    {
        am_white = board.IsWhiteToMove;
        int best_score = int.MinValue;
        Move[] moves = board.GetLegalMoves();
        Move best_move = moves[0];
        foreach (Move move in moves) 
        {
            board.MakeMove(move);
            int pos_eval = evaluate(board);
            if (pos_eval > best_score)
            {
                best_score = pos_eval;
                best_move = move;
            }
            board.UndoMove(move);
        }
        return best_move;
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
        return am_white ? eval : -1*eval;
    }
}