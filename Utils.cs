namespace trab_GB;
public static class Utils
{
    public const int LEN_BLOCK = 10;
    public const int NUM_BLOCKS_INT = 3;
    public const int NUM_MAX_BLOCKS_IND = 3;
    public const int LEN_TOTAL = LEN_BLOCK * (NUM_BLOCKS_INT + NUM_MAX_BLOCKS_IND);
    
    public static string Bin(int num)
    {
        string binary = "";
        for (int i = 31; i >= 0; i--)
        {
            int bit = num & (1 << i);
            binary += bit == 0 ? "0" : "1"; 
        }
        return binary.TrimStart('0');
    }

    public static string BinToStrDireito(int dir)
    {
        return Convert.ToString(dir, 2).PadLeft(3, '0');
    }
}

