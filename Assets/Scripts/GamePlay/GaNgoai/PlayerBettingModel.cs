public enum ETypeLayingBetting
{
    None = -1,
    Rong = 0,
    Hep = 1,
    RongHep = 2,
}
public class PlayerBettingModel
{
    private PlayerControllerChan player;

    public PlayerControllerChan Player
    {
        get { return player; }
        set { player = value; }
    }
	private int cardId;
    private bool? isWinner = null;

    public bool? IsWinner
    {
        get { return isWinner; }
        set { isWinner = value; }
    }

	public int CardId {
		get {
			return cardId;
		}
		set {
			cardId = value;
		}
	}
	private long chipBetting;

	public long ChipBetting {
		get {
			return chipBetting;
		}
		set {
			if(value == chipBetting) return;
			chipBetting = value;
		}
	}

	private string typeLaying;
	public string TypeLaying {
		get {
			return typeLaying;
		}
		set {
			if(value == typeLaying) return;
			typeLaying = value;
		}
	}
    private ETypeLayingBetting eTypeLaying;

    public ETypeLayingBetting ETypeLaying
    {
        get { return eTypeLaying; }
        set { eTypeLaying = value; }
    }
}


