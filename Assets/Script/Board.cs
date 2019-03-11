﻿using System.Collections.Generic;
using System.Linq;

namespace Tomino
{
    public class Board
    {
        public readonly int width;
        public readonly int height;
        public List<Block> Blocks { get; private set; } = new List<Block>();

        public int Top
        {
            get
            {
                return height - 1;
            }
        }

        public Board(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        public bool HasCollisions()
        {
            return HasBoardCollisions() || HasBlockCollisions();
        }

        public bool HasBlockCollisions()
        {
            var allPositions = Blocks.Select(block => block.Position);
            var uniquePositions = new HashSet<Position>(allPositions);
            return allPositions.Count() != uniquePositions.Count();
        }

        public bool HasBoardCollisions()
        {
            return Blocks.Find(CollidesWithBoard) != null;
        }

        public bool CollidesWithBoard(Block block)
        {
            return block.Position.Row < 0 ||
                   block.Position.Row >= height ||
                   block.Position.Column < 0 ||
                   block.Position.Column >= width;
        }

        override public int GetHashCode()
        {
            return Blocks.Aggregate(0, (hash, block) =>
            {
                var row = block.Position.Row;
                var column = block.Position.Column;
                var offset = width * height * (int)block.Type;
                var blockHash = offset + row * width + column;
                return hash + blockHash;
            });
        }

        public void Add(Piece piece)
        {
            var offsetRow = Top - piece.Top;
            var offsetCol = (width - piece.Width) / 2;

            foreach (var block in piece.blocks)
            {
                block.MoveBy(offsetRow, offsetCol);
            }

            Blocks.AddRange(piece.blocks);
        }

        public void MoveLeft(Piece piece) => Move(piece, 0, -1);

        public void MoveRight(Piece piece) => Move(piece, 0, 1);

        public bool MoveDown(Piece piece) => Move(piece, -1, 0);

        public bool Move(Piece piece, int rowOffset, int columnOffset)
        {
            foreach (var block in piece.blocks)
            {
                block.MoveBy(rowOffset, columnOffset);
            }

            if (HasCollisions())
            {
                foreach (var block in piece.blocks)
                {
                    block.MoveBy(-rowOffset, -columnOffset);
                }
                return false;
            }
            return true;
        }

        public void Rotate(Piece piece)
        {
            if (!piece.canRotate)
            {
                return;
            }

            Dictionary<Block, Position> piecePosition = piece.GetPositions();
            var offset = piece.blocks[0].Position;

            foreach (var block in piece.blocks)
            {
                var row = block.Position.Row - offset.Row;
                var column = block.Position.Column - offset.Column;
                block.MoveTo(-column + offset.Row, row + offset.Column);
            }

            if (HasCollisions() && !ResolveCollisionsAfterRotation(piece))
            {
                RestoreSavedPiecePosition(piece, piecePosition);
            }
        }

        bool ResolveCollisionsAfterRotation(Piece piece)
        {
            var columnOffsets = new int[] { -1, -2, 1, 2 };
            foreach (int offset in columnOffsets)
            {
                Move(piece, 0, offset);

                if (HasCollisions())
                {
                    Move(piece, 0, -offset);
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        void RestoreSavedPiecePosition(Piece piece, Dictionary<Block, Position> piecePosition)
        {
            foreach (Block block in piece.blocks)
            {
                block.MoveTo(piecePosition[block]);
            }
        }

        public void Fall(Piece piece)
        {
            while (MoveDown(piece)) { }
        }

        public int RemoveFullRows()
        {
            int rowsRemoved = 0;
            for (int row = height - 1; row >= 0; --row)
            {
                var rowBlocks = GetBlocksFromRow(row);
                if (rowBlocks.Count == width)
                {
                    Remove(rowBlocks);
                    MoveDownBlocksBelowRow(row);
                    rowsRemoved += 1;
                }
            }
            return rowsRemoved;
        }

        public void RemoveAllBlocks()
        {
            Blocks.Clear();
        }

        List<Block> GetBlocksFromRow(int row)
        {
            return Blocks.FindAll(block => block.Position.Row == row);
        }

        void Remove(List<Block> blocksToRemove)
        {
            Blocks.RemoveAll(block => blocksToRemove.Contains(block));
        }

        void MoveDownBlocksBelowRow(int row)
        {
            foreach (Block block in Blocks)
            {
                if (block.Position.Row > row)
                {
                    block.MoveBy(-1, 0);
                }
            }
        }
    }
}
