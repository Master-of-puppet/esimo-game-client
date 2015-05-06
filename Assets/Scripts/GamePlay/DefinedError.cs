using System;
using System.ComponentModel;

//public interface IDefinedError<T>
//{
//    T GetError { get; }
//    int GetCodeError(T error);
//}

public static class DefinedError
{
    public enum GameplayError
    {
        [AttributeGameError(0, "Hành động chỉ được thực hiện bởi chủ phòng")]
        ACTION_FOR_ROOM_MASTER_ONLY = 0,

        [AttributeGameError(1, "Trạng thái người chơi không hợp lệ")]
        PLAYER_STATE_INVALID = 1,

        [AttributeGameError(2, "Người chơi chưa sẵn sàng")]
        PLAYER_IS_NOT_READY = 2,

        [AttributeGameError(3, "Lá bài không tồn tại")]
        CARD_DOES_NOT_EXIST = 3,

        [AttributeGameError(4, "Bài trên tay lỗi")]
        HAND_SIZE_INVALID = 4,

        [AttributeGameError(5, "Chưa tới lượt đánh bài")]
        DISCARD_ACTION_DENIED = 5,

        [AttributeGameError(6, "Gửi bài không hợp lệ")] //Phỏm
        CARD_ADDING_TO_MELD_ERROR = 6,
		[AttributeGameError(6, "Bài đánh ra phải có cây")] //TLMN
        DISCARD_REQUIRE_CARD = 6,

        [AttributeGameError(7, "Bài đánh ra không hợp lệ")]
        DISCARD_INVALID = 7,
        [AttributeGameError(7, "Bài đánh ra không hợp lệ")] //Chắn
        DISCARD_ERROR = 7,

        [AttributeGameError(8, "Bộ bài rỗng")]
        DECK_EMPTY = 8,

        [AttributeGameError(9, "Lỗi game ... :")]
        GAME_ERROR = 9,

        [AttributeGameError(10, "Không tìm thấy người chơi")]
        PLAYER_NOT_FOUND = 10,

        [AttributeGameError(11, "Trạng thái game không hợp lệ")]
        GAME_STATE_INVALID = 11,

        [AttributeGameError(12, "Không còn chỗ để thêm robot")]
        NO_SLOT_AVAILABLE_FOR_ROBOT = 12,

        [AttributeGameError(13, "Hành động không hợp lệ")]
        INVALID_STATE = 13,

        [AttributeGameError(14, "Chưa thể ù")] //Phỏm, Chắn
        FULL_LAYING_DENIED = 14,
        
        [AttributeGameError(15, "Hành động không hợp lệ")] //Phỏm
        STEAL_CARD_ERROR = 15,
        [AttributeGameError(15, "Ăn bài không hợp lệ")] //Chắn
        STEAL_CARD_ACTION_INVALID = 15,

        [AttributeGameError(16, "Hạ phỏm lỗi")] //Phỏm
        LAY_MELD_ERROR = 16,
        [AttributeGameError(16, "Không được phép ăn")] //Chắn
        STEAL_CARD_ACTION_DENIED = 16,

        [AttributeGameError(17, "Cây ăn chưa được hạ phỏm")] //Phỏm
        STOLEN_CARD_NOT_LAYED = 17,
        [AttributeGameError(17, "Không thể chíu")] //Chắn
        CHIU_ERROR = 17,

        [AttributeGameError(18, "Bài trên tay không hợp lệ")] //Phỏm, chắn
        INVALID_HAND_SIZE = 18,

        [AttributeGameError(19, "Cây ăn không được tạo phỏm")] //Phỏm
        NO_MELD_FOR_STOLEN_CARD = 19,
    }

    public static bool IsDiscardError(int codeError)
    {
        bool isError = codeError == GetCodeError(GameplayError.DISCARD_ACTION_DENIED) || codeError == GetCodeError(GameplayError.DISCARD_INVALID);

        if (GameManager.GAME == EGame.TLMN)
            return isError || codeError == GetCodeError(GameplayError.DISCARD_REQUIRE_CARD);
        else
            return isError;
    }
    public static bool IsDiscardErrorTLMN(int codeError)
    {
        bool isError = codeError == GetCodeError(GameplayError.DISCARD_ACTION_DENIED) || codeError == GetCodeError(GameplayError.DISCARD_INVALID);
        return isError || codeError == GetCodeError(GameplayError.DISCARD_REQUIRE_CARD);
    }
    public static int GetCodeError(GameplayError enumValue)
    {
        return Utility.EnumUtility.GetAttribute<AttributeGameError>(enumValue).Code;
    }

    public class AttributeGameError : Attribute
    {
        public string Description;
        public int Code;

        public AttributeGameError(int code, string description)
        {
            this.Code = code;
            this.Description = description;
        }
    }
}

#region SERVER CONFIG
#region TLMN
//ACTION_FOR_MASTER_ONLY(0, 0, "Hành động chỉ được thực hiện bởi chủ phòng"),
//PLAYER_STATE_INVALID(1, 0, "Trạng thái người chơi không hợp lệ"), 
//PLAYER_IS_NOT_READY(2, 0, "Người chơi chưa sẵn sàng"), 
//CARD_DOES_NOT_EXIST(3, 0, "Lá bài không tồn tại"), 
//HAND_SIZE_INVALID(4, 0, "Bài trên tay lỗi"), 
//DISCARD_ACTION_DENIED(5, 0, "Không được đánh bài"), 
//DISCARD_REQUIRE_CARD(6, 0, "Bài đánh ra phải có cây "),
//DISCARD_ERROR(7, 0, "Bài đánh ra không hợp lệ"),
//DECK_EMPTY(8, 0, "Bộ bài rỗng"),
//GAME_ERROR(9, 0, "Lỗi game ... :("), 
//PLAYER_NOT_FOUND(10, 0, "Không tìm thấy người chơi"), 
//GAME_STATE_INVALID(11, 0, "Trạng thái game không hợp lệ"),
//NO_SLOT_AVAILABLE_FOR_ROBOT(12, 0, "Không còn chỗ để thêm robot");
#endregion

#region Phom
//ACTION_FOR_ROOM_MASTER_ONLY(0, 0, "Hành động chỉ được thực hiện bởi chủ phòng"),
//PLAYER_STATE_INVALID(1, 0, "Trạng thái người chơi không hợp lệ"), 
//PLAYER_IS_NOT_READY(2, 0, "Người chơi chưa sẵn sàng"), 
//CARD_DOES_NOT_EXIST(3, 0, "Lá bài không tồn tại"), 
//HAND_SIZE_INVALID(4, 0, "Bài trên tay lỗi"), 
//DISCARD_ACTION_DENIED(5, 0, "Chưa tới lượt đánh bài"), 
//CARD_ADDING_TO_MELD_ERROR(6, 0, "Gửi bài không hợp lệ"),
//DISCARD_INVALID(7, 0, "Đánh bài không hợp lệ"),
//DECK_EMPTY(8, 0, "Bộ bài rỗng"),
//GAME_ERROR(9, 0, "Lỗi game ... :("), 
//PLAYER_NOT_FOUND(10, 0, "Không tìm thấy người chơi"), 
//GAME_STATE_INVALID(11, 0, "Trạng thái game không hợp lệ"),
//NO_SLOT_AVAILABLE_FOR_ROBOT(12, 0, "Không còn chỗ để thêm robot"),
//INVALID_STATE(13, 0, "Hành động không hợp lệ"),
//FULL_LAYING_DENIED(14, 0, "Chưa thể ù"),
//STEAL_CARD_ERROR(15, 0, "Lỗi ăn bài"), 
//LAY_MELD_ERROR(16, 0, "Hạ phỏm lỗi"), 
//STOLEN_CARD_NOT_LAYED (17, 0, "Cây ăn chưa được hạ phỏm"), 
//INVALID_HAND_SIZE(18, 0, "Bài trên tay không hợp lệ"), 
//NO_MELD_FOR_STOLEN_CARD(19, 0, "Cây ăn không được tạo phỏm");
#endregion

#region Chắn
//ACTION_FOR_ROOM_MASTER_ONLY(0, 0, "Hành động chỉ được thực hiện bởi chủ phòng"),
//PLAYER_STATE_INVALID(1, 0, "Trạng thái người chơi không hợp lệ"), 
//PLAYER_IS_NOT_READY(2, 0, "Người chơi chưa sẵn sàng"), 
//CARD_DOES_NOT_EXIST(3, 0, "Lá bài không tồn tại"), 
//HAND_SIZE_INVALID(4, 0, "Bài trên tay lỗi"), 
//DISCARD_ACTION_DENIED(5, 0, "Không được đánh bài"), 
//DISCARD_ERROR(7, 0, "Bài đánh ra không hợp lệ"),
//DECK_EMPTY(8, 0, "Bộ bài rỗng"),
//GAME_ERROR(9, 0, "Lỗi game ... :("), 
//PLAYER_NOT_FOUND(10, 0, "Không tìm thấy người chơi"), 
//GAME_STATE_INVALID(11, 0, "Trạng thái game không hợp lệ"),
//NO_SLOT_AVAILABLE_FOR_ROBOT(12, 0, "Không còn chỗ để thêm robot"),
//INVALID_STATE(13, 0, "Hành động không hợp lệ"),
//STEAL_CARD_ACTION_DENIED(14, 0, "Không được phép ăn"),
//STEAL_CARD_ACTION_INVALID(15, 0, "Ăn bài không hợp lệ"),
//FULL_LAYING_DENIED(16, 0, "Không được phép ù"),
//CHIU_ERROR(17, 0, "Không thể chíu"),
//INVALID_HAND_SIZE(18, 0, "Bài trên tay không hợp lệ"), ;
#endregion
#endregion