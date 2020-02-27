using System;

namespace clox
{
    public class Token
    {
        readonly TokenType type;
        readonly String lexeme;
        readonly Object literal;
        readonly int line;

        public Token(TokenType type, String lexeme, Object literal, int line)
        {
            this.type = type;
            this.lexeme = lexeme;
            this.literal = literal;
            this.line = line;
        }

        public String toString()
        {
            return type + " " + lexeme + " " + literal;
        }
    }
}