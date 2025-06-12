using UnityEngine;
using System;
using System.Collections.Generic;
public class Parser : MonoBehaviour
{
    public MainScript main;
    public Interpreter interpreter;
    public  int line;
    public  bool ismethod = false;
    public  bool posiblevar = false;
    public List<string> Parsing(string text)
    {
        List<string> tokens = new List<string>();
        string auxstr = "";
        for (int i = 0; i < text.Length; i++)
        {
            //deleting spaces
            if (text[i] == ' ' || text[i] == '\t' || text[i] == '\n')
            {
                if (ismethod) continue;
                if (auxstr != "" )
                {
                    tokens.Add(auxstr);
                    auxstr = "";
                }
                continue;
            }
            //strings between []
            if (text[i] == '[')
            {
                while (text[i] != ']')
                {
                    auxstr += text[i];
                    i++;
                }
                auxstr += text[i];
                tokens.Add(auxstr);
                auxstr = "";
                continue;
            }
            //strings between ()
            if (text[i] == '(')
            {
                if (ismethod) auxstr += '(';
                int contParenthesis = 0;
                i++;
                while (text[i] != ')' || contParenthesis != 0)
                {
                    if (text[i] == '(')
                    {
                        auxstr += text[i];
                        i++;
                        contParenthesis++;
                        continue;
                    }
                    if (text[i] == ')')
                    {
                        auxstr += text[i];
                        i++;
                        contParenthesis--;
                        continue;
                    }
                    if (text[i] == ' ' || text[i] == '\t')
                    {
                        i++;
                        continue;
                    }
                    if (text[i] == '"')
                    {
                        i++;
                        while (text[i] != '"')
                        {
                            auxstr += text[i];
                            i++;
                        }
                        i++;
                        continue;
                    }
                    auxstr += text[i];
                    i++;
                }
                if (ismethod) auxstr += ')';
                ismethod = false;
                if (auxstr != "") tokens.Add(auxstr);
                auxstr = "";

                continue;
            }
            //end of text
            char nextchar = ' ';
            if (i != text.Length - 1) nextchar = text[i + 1];

            //particular case || and &&
            if ((text[i] == '&' && nextchar == '&') || (text[i] == '|' && nextchar == '|'))
            {
                tokens.Add(auxstr);
                auxstr = "";
                tokens.Add(Char.ToString(text[i]) + text[i]);
                i++;
                continue;
            }

            auxstr += text[i];

            //variable declaration
            if (auxstr == "<-")
            {
                tokens.Add(auxstr);
                auxstr = "";
                i++;
                while (i < text.Length && text[i] != '\n')
                {
                    if (text[i] == ' ' || text[i] == '\t')
                    {
                        i++;
                        continue;
                    }
                    auxstr += text[i];
                    i++;
                }
                tokens.Add(auxstr);
                auxstr = "";
                continue;
            }
            //method
            if (interpreter.IsMethod(auxstr))
            {
                ismethod = true;
                continue;
            }
            //dictionary of tokens
            if (Comparation(auxstr, nextchar))
            {
                tokens.Add(auxstr);
                posiblevar = false;
                auxstr = "";
            }
        }
        if (auxstr != "") tokens.Add(auxstr);
        List<string> filteredTokens = new List<string>();
        for (int i = 0; i < tokens.Count; i++)
        {
            if (tokens[i].Trim() != "") filteredTokens.Add(tokens[i].Trim());
        }
        return filteredTokens;
    }
    public  List<string> MethodParsing(string text)
    {
        List<string> tokens = new List<string>();
        string auxstr = "";
        
        for (int i = 0; i < text.Length; i++)
        {
            //copy the name method
            if (i == 0)
            {
                for (int j = 0; j < text.Length; j++)
                {
                    if (text[j] == ' ' || text[j] == '(')
                    {
                        tokens.Add(auxstr);
                        auxstr = "";
                        break;
                    }
                    auxstr += text[i];
                    i++;
                }
            }            
            //deleting spaces
                if (text[i] == ' ' || text[i] == '\t' || text[i] == '\n' || text[i] == ',')
                {
                    if (auxstr != "")
                    {
                        tokens.Add(auxstr);
                        auxstr = "";
                    }
                    continue;
                }
            //strings between ()
            if (text[i] == '(')
            {
                int contParenthesis = 0;
                i++;
                while (text[i] != ')' || contParenthesis != 0)
                {
                    if (text[i] == '(')
                    {
                        auxstr += text[i];
                        i++;
                        contParenthesis++;
                        continue;
                    }
                    if (text[i] == ')')
                    {
                        auxstr += text[i];
                        i++;
                        contParenthesis--;
                        continue;
                    }
                    if (text[i] == ' ' || text[i] == '\t')
                    {
                        i++;
                        continue;
                    }
                    if (text[i] == '"')
                    {
                        auxstr = "";
                        i++;
                        while (text[i] != '"')
                        {
                            auxstr += text[i];
                            i++;
                        }
                        i++;
                        continue;
                    }
                    if (text[i] == ',')
                    {
                        if (contParenthesis != 0)
                        {
                            main.log.text = "ERROR, PARÉNTESIS INCOMPLETO!!!!  line " + line;
                            interpreter.error = true;
                        }    
                        tokens.Add(auxstr);
                        auxstr = "";
                        i++;
                        continue;
                    }
                    auxstr += text[i];
                    i++;
                }
                if (auxstr != "") tokens.Add(auxstr);
                auxstr = "";
                continue;
            }
            auxstr += text[i];
        }
        return tokens;
    }
    public  List<string> BoolParsing(string token)
    {
        List<string> toks = new List<string> ();
        int deep = 0;
        string auxstr = "";
        if (token[0] == '&' || token[0] == '|')
        {
            Console.WriteLine("ERROR!!!! INCORRECT BOOLEAN EXPRESSION line " + line);
            interpreter.error = true;
            return toks;
        }
        for (int i = 0; i < token.Length; i++)
        {
            if (token[i] == '(')
            {
                deep++;
                auxstr += token[i];
                continue;
            }
            if (token[i] == ')')
            {
                deep--;
                auxstr += token[i];
                continue;
            }
            if (i >= token.Length - 2 && (token[i] == '&' || token[i] == '|'))
            {
                Console.WriteLine("ERROR!!!! INCORRECT BOOLEAN EXPRESSION line " + line);
                interpreter.error = true;
                return toks;
            }
            if (((token[i] == '&' && token[i + 1] == '&') || (token[i] == '|' && token[i + 1] == '|')) && deep == 0)
            {
                if (auxstr != "") toks.Add(auxstr);
                auxstr = "";
                toks.Add(Char.ToString(token[i]) + token[i + 1]);
                i++;
                continue;
            }
            auxstr += token[i];
        }
        //last token
        if(auxstr != "") toks.Add(auxstr);
        return toks;
    }
    public  List<string> PredicateParsing(string token)
    {
        List<string> toks = new List<string> ();
        int deep = 0;
        string auxstr = "";
        if (token[0] == '>' || token[0] == '<' || token[0] == '=' || token[0] == '!')
        {
            Console.WriteLine("ERROR!!!! INCORRECT BOOLEAN EXPRESSION line " + line);
            interpreter.error = true;
            return toks;
        }
        for (int i = 0; i < token.Length; i++)
        {
            if (token[i] == '(')
            {
                deep++;
                auxstr += token[i];
                continue;
            }
            if (token[i] == ')')
            {
                deep--;
                auxstr += token[i];
                continue;
            }
            if (token[token.Length - 1] == '>' || token[token.Length - 1] == '<' || token[token.Length - 1] == '=')
            {
                Console.WriteLine("ERROR!!!! INCORRECT BOOLEAN EXPRESSION line " + line);
                interpreter.error = true;
                return toks;
            }
            if ((token[i] == '<' || token[i] == '>' || token[i] == '=' || (token[i] == '!' && token[i+1] == '=')) && deep == 0)
            {
                if (auxstr != "") toks.Add(auxstr);
                if (token[i + 1] == '=')
                {
                    toks.Add(Char.ToString(token[i]) + token[i + 1]);
                    i++;
                }
                else
                {
                    toks.Add(Char.ToString(token[i]));
                }
                auxstr = "";
                continue;
            }
            auxstr += token[i];
        }
        //last token
        if(auxstr != "") toks.Add(auxstr);
        return toks;
    }
    public  bool Comparation(string auxstr, char nextchar)
    {
        //functions
        if (nextchar == ' ' || nextchar == '(')
        {
            if (Enum.IsDefined(typeof(Method), auxstr)) return false;
        }
        if ((nextchar == ' ' || nextchar == '[') && auxstr == "GoTo") return true;

        //numbers
        if (int.TryParse(auxstr, out int num) && !posiblevar)
        {
            if (int.TryParse(nextchar.ToString(), out int numm)) return false;
            else return true;
        }

        //colors
        if (Enum.IsDefined(typeof(CellColor), auxstr) && Comparation(Convert.ToString(nextchar), ' ')) return true;

        //operators
        if (auxstr == "+" ||
            auxstr == "-" ||
            auxstr == "*" ||
            auxstr == "**" ||
            auxstr == "/" ||
            auxstr == "%")
        {
            if (auxstr == "*" && nextchar == '*') return false;
            return true;
        }

        //logic operators
        if (auxstr == "<" ||
            auxstr == ">" ||
            auxstr == "=" ||
            auxstr == "<-" ||
            auxstr == "==" ||
            auxstr == "!=" ||
            auxstr == "<=" ||
            auxstr == ">=" ||
            auxstr == "&&" ||
            auxstr == "||")
        {
            if (auxstr == "<" && nextchar == '-') return false;
            if (nextchar == '=') return false;
            return true;
        }
        if (auxstr == "!" && nextchar == '=') return false;
        //booleans
        if ((auxstr == "true" || auxstr == "false") && !(Char.IsLetterOrDigit(nextchar) || nextchar == '_')) return true;
        //another cases
        if (auxstr == ")" || auxstr == "(") return true;
        //variables
        if (posiblevar)
        {
            posiblevar = false;
            return false;
        }
        posiblevar = true;
        if (Comparation(Convert.ToString(nextchar), ' ')) return true;
        posiblevar = false;
        //is nothing
        return false;
    }
}