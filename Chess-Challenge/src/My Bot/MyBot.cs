using ChessChallenge.API;
using System.ComponentModel.DataAnnotations;
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
            int pos_eval = -negamax(board, search_depth-1, int.MinValue, int.MaxValue, am_white*-1);
            if (pos_eval > best_score)
            {
                best_score = pos_eval;
                best_move = move;
            }
            board.UndoMove(move);
        }
        return best_move;
    }

    public int negamax(Board board, int depth, int alpha, int beta, int color) //white is 1, black is -1
    {
        if (depth == 0 || board.IsInCheckmate())
            return color * evaluate(board);
        int score = int.MinValue;
        Move[] moves = board.GetLegalMoves();
        foreach (Move move in moves)
        {
            board.MakeMove(move);
            int search_score = -negamax(board, depth - 1, -beta, -alpha, -color);
            board.UndoMove(move);
            score = max(score, search_score); //max of current best score and new score
            alpha = max(alpha, score);
            if (alpha >= beta)  //prune the current node, we will never reach it
                break;
        }
        return score;
    }

    public int evaluate(Board board)
    {
        //reused local vars (ik the compiler can maybe handle the optimization but taking no chances)
        //==========

        //general
        PieceList[] piece_lists = board.GetAllPieceLists();     //getting pieces
        PieceList piece_type_list;

        //Material
        int white_points = 0;  //material counts (centipawns)
        int black_points = 0;

        //Mobility
        PieceType piece_type;
        ulong white_piece_attacks_bb;
        ulong black_piece_attacks_bb;
        int white_mobility = 0;
        int black_mobility = 0;

        //==========
        //reused local vars

        //checkmate
        //=========
        int white_to_move = board.IsWhiteToMove ? 0 : 1;
        int black_to_move = board.IsWhiteToMove ? 1 : 0;
        int is_checkmate = board.IsInCheckmate() ? 1 : 0;
        int white_checkmates = 10000 * white_to_move * is_checkmate;
        int black_checkmates = 10000 * black_to_move * is_checkmate;
        //=========
        //checkmate

        for (int i = 0; i < 12; i++) //iterating over all pieces
        {
            //vars set for every new piece type
            piece_type_list = piece_lists[i];
            int piece_count = piece_type_list.Count;
            piece_type = piece_lists[i].TypeOfPieceInList;

            //Material
            //========
            if (piece_type != 6) //skip king values, since they are N/A
            {
                if (i < 6) //white pieces
                {
                    white_points += piece_count * piece_val_arr[i];
                }
                else //black pieces
                {
                    black_points += piece_count * piece_val_arr[i - 6];
                }
            }
            //========
            //Material

            //Mobility
            //========
            foreach (Piece piece in piece_type_list)
            {
                white_piece_attacks_bb = GetPieceAttacks(piece_type, piece.Square, board, 1);
                black_piece_attacks_bb = GetPieceAttacks(piece_type, piece.Square, board, 0);

                white_mobility += GetNumberOfSetBits(white_piece_attacks_bb); //TODO:: set and calibrate a multiplier on squares attacked to get a mobility score
                black_mobility += GetNumberOfSetBits(black_piece_attacks_bb);
            }
            //========
            //Mobility
        }


        

        

        int eval = (white_points - black_points) + (white_checkmates - black_checkmates);
        return eval;
    }

    public int max(int a, int b)
    {
        return (a > b) ? a : b;
    }
}