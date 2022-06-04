using SQLite;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Guppi_Memorise
{
    public class Card
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Indexed]
        public int DeckId { get; set; }
        public string Title { get; set; } = "Новая карточка";
        public string Text { get; set; } = "Текст карточки";
        public int Rating { get; set; } = 0;
    }
    public class Deck
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; } = "Новая колода";
    }
    public class Text
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Body { get; set; }
        public string Time { get; set; }
    }
    public class UserStats
    {
        // public int DecksCreated { get; set; }
        // public int FlashCardsCreated { get; set; }
        // public int TextsLearned { get; set; }
        // public string FastestLearningTime { get; set; }
        // Those are fetched from the DB
        public int TextsEntered { get; set; } = 0;
    }
    public static class DB
    {
        static SQLiteAsyncConnection db = null;
        public static async Task Init()
        {
            if (db != null) return;
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "mdb.db3");
            db = new SQLiteAsyncConnection(dbPath);
            await db.CreateTableAsync<Card>();
            await db.CreateTableAsync<Deck>();
            await db.CreateTableAsync<Text>();
            await db.CreateTableAsync<UserStats>();
        }
        public static async Task<List<Deck>> FetchDecks()
        {
            await Init();
            var decks = await db.Table<Deck>().ToListAsync();
            return decks;
        }
        public static async Task<List<Card>> FetchCards(Deck deck)
        {
            await Init();
            var query = await db.Table<Card>().Where(e => e.DeckId == deck.Id).ToListAsync();
            return query;
        }
        public static async Task<int> CountCardsInDeck(Deck deck)
        {
            await Init();
            // Needs to be tested
            var count = await db.Table<Card>().Where(e => e.DeckId == deck.Id).CountAsync();
            return count;
        }
        public static async Task<int> CountCardsTotal()
        {
            await Init();
            var count = await db.Table<Card>().CountAsync();
            return count;
        }
        public static async Task<int> CountDecksTotal()
        {
            await Init();
            var count = await db.Table<Deck>().CountAsync();
            return count;
        }
        public static async Task<int> CountTextsTotal()
        {
            await Init();
            var count = await db.Table<Text>().CountAsync();
            return count;
        }
        public static async Task AddDeck(Deck deck)
        {
            await Init();
            await db.InsertAsync(deck);
        }
        public static async Task AddCard(Deck deck, Card card)
        {
            await Init();
            card.DeckId = deck.Id;
            await db.InsertAsync(card);
        }
        public static async Task RemoveCard(Card card)
        {
            await Init();
            await db.DeleteAsync<Card>(card.Id);
        }
        public static async Task RemoveDeck(Deck deck)
        {
            await Init();
            await db.DeleteAsync<Deck>(deck.Id);
            await db.ExecuteAsync($"DELETE FROM Card WHERE DeckId = {deck.Id}");
        }
        public static async Task UpdateDeck(Deck deck)
        {
            await Init();
            await db.UpdateAsync(deck);
        }
        public static async Task UpdateCard(Card card)
        {
            await Init();
            await db.UpdateAsync(card);
        }
        public static async Task AddText(Text text)
        {
            await Init();
            await db.InsertAsync(text);
        }
        public static async Task UpdateText(Text text)
        {
            await Init();
            await db.UpdateAsync(text);
        }
        public static async Task<List<Text>> FetchTexts()
        {
            await Init();
            var texts = await db.Table<Text>().ToListAsync();
            return texts;
        }
        public static async Task PurgeDecks()
        {
            await Init();
            await db.DeleteAllAsync<Deck>();
        }
        public static async Task PurgeCards()
        {
            await Init();
            await db.DeleteAllAsync<Card>();
        }
        public static async Task PurgeTexts()
        {
            await Init();
            await db.DeleteAllAsync<Text>();
        }
        public static async Task PurgeInitUserStats()
        {
            await Init();
            await db.DeleteAllAsync<UserStats>();
            var cleanUserStats = new UserStats();
            await db.InsertAsync(cleanUserStats);
        }
        public static async Task AddDummyTexts()
        {
            await Init();
            await db.InsertAsync(new Text { Body = "Я помню чудное мгновенье:\nПередо мной явилась ты,\nКак мимолетное виденье,\nКак гений чистой красоты.\n\nВ томленьях грусти безнадежной,\nВ тревогах шумной суеты,\nЗвучал мне долго голос нежный\nИ снились милые черты.", Time = "00:12:33" });
            await db.InsertAsync(new Text { Body = "Я помню чудное мгновенье:\nПередо мной явилась ты,\nКак мимолетное виденье,\nКак гений чистой красоты.\n\nВ томленьях грусти безнадежной,\nВ тревогах шумной суеты,\nЗвучал мне долго голос нежный\nИ снились милые черты.", Time = "01:24:48" });
            await db.InsertAsync(new Text { Body = "111111111111111111111111\nединчика!!!\n1111111111111111111111111111111111111111111111\n1111111111111111111111111111111111111111111111111\n1111111111111111111111111111111\n1111111111111 111111111111\n11111111111111111111111111\n1111111111111111111111111111\n1111111111111111111111\n1111111111111111111111\n1111111111 111111111111111111111111111\n11111111111111111111111111111111111111111111\n1111111111111111111111111111111111111111111111111111111111\n11111111111111 единичка11", Time = "51:53:24" });
        }
        public static async Task<string> FetchMinimalLearningTime()
        {
            await Init();
            string curTime = "--:--:--";
            var texts = await db.Table<Text>().ToListAsync();
            if (texts.Count == 0)
            {
                return curTime;
            }
            else
            {
                TimeSpan minTimeSpan = TimeSpan.Parse(texts[0].Time);
                foreach (var text in texts)
                {
                    TimeSpan curTimeSpan = TimeSpan.Parse(text.Time);
                    if (curTimeSpan < minTimeSpan)
                    {
                        minTimeSpan = curTimeSpan;
                    }
                }
                curTime = string.Format("{0:00}:{1:00}:{2:00}", minTimeSpan.Hours, minTimeSpan.Minutes, minTimeSpan.Seconds);
                return curTime;
            }
        }
        public static async Task<int> FetchNumberOfTextsEntered()
        {
            await Init();
            var userStats = await db.Table<UserStats>().FirstOrDefaultAsync();
            if (userStats == null)
            {
                await PurgeInitUserStats();
                userStats = await db.Table<UserStats>().FirstOrDefaultAsync();
            }
            int number = userStats.TextsEntered;
            return number;
        }
        public static async Task IncreaseNumberOfTextsEntered()
        {
            await Init();
            var userStats = await db.Table<UserStats>().FirstOrDefaultAsync();
            if (userStats is null)
            {
                await PurgeInitUserStats();
                userStats = await db.Table<UserStats>().FirstOrDefaultAsync();
            }
            userStats.TextsEntered++;
            await db.DeleteAllAsync<UserStats>();
            await db.InsertAsync(userStats);
        }
        public static async Task PurgeUserData()
        {
            await Init();
            await PurgeDecks();
            await PurgeCards();
            await PurgeTexts();
            await PurgeInitUserStats();
        }
    }
}