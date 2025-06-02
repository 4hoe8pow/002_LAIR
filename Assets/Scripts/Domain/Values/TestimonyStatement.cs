namespace Domain
{
    public enum TestimonyStatement
    {
        UpIsFox,
        UpIsOwl,
        DownIsFox,
        DownIsOwl,
        LeftIsFox,
        LeftIsOwl,
        RightIsFox,
        RightIsOwl
    }

    public static class TestimonyStatementExtensions
    {
        public static string ToJapaneseString(this TestimonyStatement statement)
        {
            return statement switch
            {
                TestimonyStatement.UpIsFox => "上は狐だ",
                TestimonyStatement.UpIsOwl => "上は梟だ",
                TestimonyStatement.DownIsFox => "下は狐だ",
                TestimonyStatement.DownIsOwl => "下は梟だ",
                TestimonyStatement.LeftIsFox => "左は狐だ",
                TestimonyStatement.LeftIsOwl => "左は梟だ",
                TestimonyStatement.RightIsFox => "右は狐だ",
                TestimonyStatement.RightIsOwl => "右は梟だ",
                _ => "?"
            };
        }
    }
}
