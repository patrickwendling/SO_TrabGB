namespace trab_GB;
public static class Utils
{
    public const int LEN_BLOCK = 10;
    public const int NUM_BLOCKS_INT = 3;
    public const int NUM_MAX_BLOCKS_IND = 3;
    public const int LEN_TOTAL = LEN_BLOCK * (NUM_BLOCKS_INT + NUM_MAX_BLOCKS_IND);

    public static string BinToStr(int bin)
    {
        return Convert.ToString(bin, 2).PadLeft(3, '0');
    }

    public static byte[] StringToByteArray(string hex)
    {
        hex = hex.Replace("-", "");
        byte[] byteArray = new byte[hex.Length / 2];

        for (int i = 0; i < byteArray.Length; i++)
        {
            byteArray[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
        }

        return byteArray;
    }

    public static string MontaPossivelNome(Inode inode, string nome)
    {
        return inode.absName == "/" ? inode.absName + nome : "/" + nome;
    }

    public static string MontaNomePadraoBlocoIndireto(string nomeAbs, int idxBocInd)
    {
        return nomeAbs + ".blocoInd" + idxBocInd.ToString();
    }
}

