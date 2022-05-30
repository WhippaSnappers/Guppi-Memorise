﻿using SQLite;
using System.Collections.Generic;
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
        public int DecksCreated { get; set; }
        public int FlashCardsCreated { get; set; }
        public int TextsEntered { get; set; }
        public int TextsLearned { get; set; }
        public string FastestLearningTime { get; set; }
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
        }
        public static async Task DropTableDeck()
        {
            await db.DeleteAllAsync<Deck>();
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
        public static async Task<int> CountCards(Deck deck)
        {
            await Init();
            // Needs to be tested
            var query = await db.Table<Card>().Where(e => e.DeckId == deck.Id).CountAsync();
            return query;
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
    }
}