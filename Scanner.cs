using System;
using System.Collections.Generic;
using System.Threading;

namespace clox
{
    public class Scanner
    {
        private readonly String _source;
        private List<Token> tokens = new List<Token>();
        private int _start = 0;
        private int _current = 0;
        private int _line = 1;
        private static readonly Dictionary<string, TokenType?> keywords = new Dictionary<string, TokenType?>()
        {
            {"and",    TokenType.AND},
            {"class",  TokenType.CLASS},                     
            {"else",   TokenType.ELSE},                      
            {"false",  TokenType.FALSE},                     
            {"for",    TokenType.FOR},                       
            {"fun",    TokenType.FUN},                       
            {"if",     TokenType.IF},                        
            {"nil",    TokenType.NIL},                       
            {"or",     TokenType.OR},                        
            {"print",  TokenType.PRINT},                     
            {"return", TokenType.RETURN},                    
            {"super",  TokenType.SUPER},                     
            {"this",   TokenType.THIS},                      
            {"true",   TokenType.TRUE},                      
            {"var",    TokenType.VAR},                       
            {"while",  TokenType.WHILE},          
        };
                             
                

        public Scanner(String source)
        {
            this._source = source;
        }

        public List<Token> scanTokens()
        {
            while (!isAtEnd())
            {
                _start = _current;
                scanToken();
            }

            tokens.Add(new Token(TokenType.EOF, "", null, _line));
            return tokens;
        }

        private bool isAtEnd()
        {
            return _current >= _source.Length;
        }

        private void scanToken()
        {
            char c = advance();
            switch (c) {                                 
                case '(': addToken(TokenType.LEFT_PAREN); break;     
                case ')': addToken(TokenType.RIGHT_PAREN); break;    
                case '{': addToken(TokenType.LEFT_BRACE); break;     
                case '}': addToken(TokenType.RIGHT_BRACE); break;    
                case ',': addToken(TokenType.COMMA); break;          
                case '.': addToken(TokenType.DOT); break;            
                case '-': addToken(TokenType.MINUS); break;          
                case '+': addToken(TokenType.PLUS); break;           
                case ';': addToken(TokenType.SEMICOLON); break;      
                case '*': addToken(TokenType.STAR); break; 
                case '!': addToken(match('=') ? TokenType.BANG_EQUAL : TokenType.BANG); break;      
                case '=': addToken(match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL); break;    
                case '<': addToken(match('=') ? TokenType.LESS_EQUAL : TokenType.LESS); break;      
                case '>': addToken(match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER); break;
                case '/':                                                       
                    if (match('/')) {                                             
                        // A comment goes until the end of the line.                
                        while (peek() != '\n' && !isAtEnd()) advance();             
                    } else {                                                      
                        addToken(TokenType.SLASH);                                            
                    }                                                             
                    break;         
                
                case ' ':
                case '\r':
                case '\t':
                    // Ignore whitespace.                      
                    break;
                case '\n':                                   
                    _line++;                                    
                    break;
                
                case '"': handleString(); break;
                case 'o':
                    if (peek() == 'r') {
                        addToken(TokenType.OR);
                    }
                    break;
                
                default:
                    if (isDigit(c)) {                          
                        number();                                
                    } else if (isAlpha(c)) {                   
                        identifier();                            
                    } else {                                   
                        Clox.error(_line, "Unexpected character.");
                    }  
                    break;
            }                  
        }

        private void identifier()
        {
            while (isAlphaNumeric(peek())) advance();
            String text = _source.Substring(_start, _current - _start);

            TokenType? type = null; 
            keywords.TryGetValue(text, out type);
            if (type == null) type = TokenType.IDENTIFIER;
            addToken((TokenType)type);
        }

        private bool isAlphaNumeric(char c)
        {
            return isAlpha(c) || isDigit(c);      
        }

        private bool isAlpha(in char c)
        {
            return (c >= 'a' && c <= 'z') ||      
                   (c >= 'A' && c <= 'Z') ||      
                   c == '_';
            
        }

        private void number() {                                     
            while (isDigit(peek())) advance();

            // Look for a fractional part.                            
            if (peek() == '.' && isDigit(peekNext())) {               
                // Consume the "."                                      
                advance();                                              

                while (isDigit(peek())) advance();                      
            }                                                         

            addToken(TokenType.NUMBER,                                          
                Double.Parse(_source.Substring(_start, _current - _start)));
        }

        private char peekNext()
        {
            if (_current + 1 >= _source.Length) return '\0';
            return _source[_current + 1];    
        }

        private bool isDigit(char c) {
            return c >= '0' && c <= '9';   
        } 

        
        private void handleString()
        { 
            while (peek() != '"' && !isAtEnd()) {                   
                if (peek() == '\n') _line++;                           
                advance();                                            
            }
            // Unterminated string.                                 
            if (isAtEnd()) {                                        
                Clox.error(_line, "Unterminated string.");              
                return;                                               
            }

            advance();
            // Trim the surrounding quotes.                         
            String value = _source.Substring(_start + 1, (_current - 2 - _start));
            addToken(TokenType.STRING, value);        
        }

        private char peek()
        {
            if (isAtEnd()) return '\0';
            return _source[_current];
        }

        private bool match(char expected)
        {
            if (isAtEnd()) return false;
            if (_source[_current] != expected) return false;
            
            _current++;
            return true;
        }

        private char advance()
        {
            _current++;
            return _source[_current - 1];
        }
        private void addToken(TokenType type) {                
            addToken(type, null);                                
        }                                                      

        private void addToken(TokenType type, Object literal) {
            String text = _source.Substring(_start, _current - _start);      
            tokens.Add(new Token(type, text, literal, _line));    
        }                
    }
}