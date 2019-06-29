﻿using NUnit.Framework;
using Tomino;

public class GameTests
{
    StubInput input;
    StubPieceProvider pieceProvider;
    Board board;
    Game game;

    [SetUp]
    public void Initialize()
    {
        input = new StubInput();
        pieceProvider = new StubPieceProvider();
        board = new Board(10, 20, pieceProvider);
        game = new Game(board, input);
        game.Start();
    }

    [Test]
    public void CreatesNewPieceWhenTheGameStarts()
    {
        Assert.IsNotEmpty(board.Blocks);
    }

    [TestCase(PlayerAction.MoveLeft, 0, -1)]
    [TestCase(PlayerAction.MoveRight, 0, 1)]
    [TestCase(PlayerAction.MoveDown, -1, 0)]
    public void MovesPiece(PlayerAction action, int rowOffset, int columnOffset)
    {
        var positions = board.GetBlockPositions();

        UpdateGameWithAction(action);

        foreach (Block block in board.Blocks)
        {
            var start = positions[block];
            var end = block.Position;
            Assert.AreEqual(end.Row, start.Row + rowOffset);
            Assert.AreEqual(end.Column, start.Column + columnOffset);
        }
    }

    [Test]
    public void MovesPieceDownWhenUpdating()
    {
        var positions = board.GetBlockPositions();
        game.Update(10);

        foreach (Block block in board.Blocks)
        {
            Assert.AreEqual(block.Position.Row, positions[block].Row - 1);
        }
    }

    [Test]
    public void RotatesPiece()
    {
        var secondBlockPositions = new Position[]
        {
            new Position(2, 1),
            new Position(1, 2),
            new Position(0, 1),
            new Position(1, 0),
            new Position(2, 1)
        };

        pieceProvider = new StubPieceProvider();
        board = new Board(3, 3, pieceProvider);
        game = new Game(board, input);
        game.Start();

        for (var i = 1; i < secondBlockPositions.Length; ++i)
        {
            UpdateGameWithAction(PlayerAction.Rotate);
            var secondBlock = board.Blocks[1];

            Assert.AreEqual(secondBlockPositions[i].Row, secondBlock.Position.Row);
            Assert.AreEqual(secondBlockPositions[i].Column, secondBlock.Position.Column);
        }
    }

    [Test]
    public void AddsNewPieceAfterCurrentPieceFallsDown()
    {
        var blocksCount = board.Blocks.Count;

        UpdateGameWithAction(PlayerAction.Fall);

        blocksCount += pieceProvider.piece.blocks.Length;

        Assert.AreEqual(blocksCount, board.Blocks.Count);
    }

    [TestCase(PlayerAction.MoveLeft, false)]
    [TestCase(PlayerAction.MoveRight, false)]
    [TestCase(PlayerAction.MoveLeft, true)]
    [TestCase(PlayerAction.MoveRight, true)]
    public void HandlesCollisions(PlayerAction action, bool blockCollision)
    {
        if (blockCollision)
        {
            for (int row = 0; row < board.height; ++row)
            {
                var leftPosition = new Position(row, 0);
                var rightPostion = new Position(row, board.width - 1);
                board.Blocks.Add(new Block(leftPosition, PieceType.I));
                board.Blocks.Add(new Block(rightPostion, PieceType.I));
            }
        }

        for (int i = 0; i < 50; ++i)
        {
            UpdateGameWithAction(action);
            UpdateGameWithAction(PlayerAction.Rotate);
        }

        Assert.IsFalse(board.HasCollisions());
    }

    [Test]
    public void RemovesFullRows()
    {
        board.AddFullRows(board.height / 2);
        var blocksCount = pieceProvider.piece.blocks.Length;
        UpdateGameWithAction(PlayerAction.Fall);
        blocksCount += pieceProvider.piece.blocks.Length;

        Assert.AreEqual(blocksCount, board.Blocks.Count);
    }

    [Test]
    public void FiresAnEventWhenTheGameFinishes()
    {
        var spy = new GameEventSpy();
        game.FinishedEvent += spy.OnGameFinished;

        UpdateGameWithAction(PlayerAction.Fall);

        Assert.IsTrue(spy.gameFinishedCalled);
    }

    [TestCase(1, 100)]
    [TestCase(2, 300)]
    [TestCase(3, 500)]
    [TestCase(4, 800)]
    public void UpdatesScoreWhenRowsAreCleared(int fullRowsCount, int score)
    {
        board.AddFullRows(fullRowsCount);
        game.WaitUntilPieceFallsAutomatically();

        Assert.AreEqual(score, game.Score.Value);
    }

    [Test]
    public void UpdatesScoreWhenPieceFalls()
    {
        int distance = board.FallDistance();
        UpdateGameWithAction(PlayerAction.Fall);

        Assert.AreEqual(distance * 2, game.Score.Value);
    }

    void UpdateGameWithAction(PlayerAction action)
    {
        input.action = action;
        game.Update(0);
    }
}
