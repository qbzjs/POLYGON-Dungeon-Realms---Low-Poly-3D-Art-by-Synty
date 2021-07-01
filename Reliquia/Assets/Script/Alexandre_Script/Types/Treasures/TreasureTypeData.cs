namespace AlexandreTreasures
{
    [System.Serializable]
    public class TreasureTypeData
    {
        // Ce script décrit un type spécifique des trésors, utilisé pour enregistrer le fichier (voir TreasureFileData).

        public int m_instanceId;
        public bool m_isDiscovered;

        public TreasureTypeData(int i, bool b)
        {
            m_instanceId = i;
            m_isDiscovered = b;
        }
    }
}