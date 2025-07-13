using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using yfinance.consts;
using yfinance.exceptions;

namespace yfinance.screener
{
    public abstract class QueryBase
    {
        public string Operator { get; }
        public List<object> Operands { get; }

        protected QueryBase(string op, List<object> operands)
        {
            op = op.ToUpperInvariant();

            if (operands == null || operands.Count == 0)
                throw new ArgumentException("Operands cannot be null or empty");

            switch (op)
            {
                case "IS-IN":
                    ValidateIsInOperand(operands);
                    break;
                case "OR":
                case "AND":
                    ValidateOrAndOperand(operands);
                    break;
                case "EQ":
                    ValidateEqOperand(operands);
                    break;
                case "BTWN":
                    ValidateBtwnOperand(operands);
                    break;
                case "GT":
                case "LT":
                case "GTE":
                case "LTE":
                    ValidateGtLtOperand(operands);
                    break;
                default:
                    throw new ArgumentException("Invalid Operator Value");
            }

            Operator = op;
            Operands = operands;
        }

        public abstract Dictionary<string, List<string>> ValidFields { get; }
        public abstract Dictionary<string, object> ValidValues { get; }

        private void ValidateOrAndOperand(List<object> operands)
        {
            if (operands.Count <= 1)
                throw new ArgumentException("Operand must be length longer than 1");
            if (!operands.All(e => e is QueryBase))
                throw new ArgumentException("Operand must be type QueryBase for OR/AND");
        }

        private void ValidateEqOperand(List<object> operands)
        {
            if (operands.Count != 2)
                throw new ArgumentException("Operand must be length 2 for EQ");

            string field = operands[0] as string;
            if (!ValidFields.Values.Any(fields => fields.Contains(field)))
                throw new ArgumentException($"Invalid field for {GetType().Name} \"{field}\"");

            if (ValidValues.ContainsKey(field))
            {
                var vv = ValidValues[field];
                HashSet<object> validSet;
                if (vv is Dictionary<string, List<string>> dict)
                    validSet = new HashSet<object>(dict.Values.SelectMany(x => x));
                else if (vv is IEnumerable<object> enumerable)
                    validSet = new HashSet<object>(enumerable);
                else
                    validSet = new HashSet<object> { vv };

                if (!validSet.Contains(operands[1]))
                    throw new ArgumentException($"Invalid EQ value \"{operands[1]}\"");
            }
        }

        private void ValidateBtwnOperand(List<object> operands)
        {
            if (operands.Count != 3)
                throw new ArgumentException("Operand must be length 3 for BTWN");

            string field = operands[0] as string;
            if (!ValidFields.Values.Any(fields => fields.Contains(field)))
                throw new ArgumentException($"Invalid field for {GetType().Name}");

            if (!(operands[1] is IConvertible) || !(operands[2] is IConvertible))
                throw new ArgumentException("Invalid comparison type for BTWN");
        }

        private void ValidateGtLtOperand(List<object> operands)
        {
            if (operands.Count != 2)
                throw new ArgumentException("Operand must be length 2 for GT/LT");

            string field = operands[0] as string;
            if (!ValidFields.Values.Any(fields => fields.Contains(field)))
                throw new ArgumentException($"Invalid field for {GetType().Name} \"{field}\"");

            if (!(operands[1] is IConvertible))
                throw new ArgumentException("Invalid comparison type for GT/LT");
        }

        private void ValidateIsInOperand(List<object> operands)
        {
            if (operands.Count < 2)
                throw new ArgumentException("Operand must be length 2+ for IS-IN");

            string field = operands[0] as string;
            if (!ValidFields.Values.Any(fields => fields.Contains(field)))
                throw new ArgumentException($"Invalid field for {GetType().Name} \"{field}\"");

            if (ValidValues.ContainsKey(field))
            {
                var vv = ValidValues[field];
                HashSet<object> validSet;
                if (vv is Dictionary<string, List<string>> dict)
                    validSet = new HashSet<object>(dict.Values.SelectMany(x => x));
                else if (vv is IEnumerable<object> enumerable)
                    validSet = new HashSet<object>(enumerable);
                else
                    validSet = new HashSet<object> { vv };

                for (int i = 1; i < operands.Count; i++)
                {
                    if (!validSet.Contains(operands[i]))
                        throw new ArgumentException($"Invalid EQ value \"{operands[i]}\"");
                }
            }
        }

        public virtual Dictionary<string, object> ToDict()
        {
            string op = Operator;
            List<object> ops = Operands;

            if (Operator == "IS-IN")
            {
                op = "OR";
                var field = Operands[0];
                ops = Operands.Skip(1)
                    .Select(v => (object)Activator.CreateInstance(GetType(), "EQ", new List<object> { field, v }))
                    .ToList();
            }

            return new Dictionary<string, object>
            {
                { "operator", op },
                { "operands", ops.Select(o => o is QueryBase qb ? qb.ToDict() : o).ToList() }
            };
        }

        public override string ToString()
        {
            return ToString(0);
        }

        public string ToString(int indent)
        {
            string indentStr = new string(' ', indent * 2);
            string className = GetType().Name;

            if (Operands is List<object> list)
            {
                if (list.Any(op => op is QueryBase))
                {
                    var operandsStr = string.Join(",\n",
                        list.Select(op => op is QueryBase qb
                            ? $"{indentStr}  {qb.ToString(indent + 1)}"
                            : $"{indentStr}  {op}"));
                    return $"{className}({Operator}, [\n{operandsStr}\n{indentStr}])";
                }
                else
                {
                    return $"{className}({Operator}, [{string.Join(", ", list)}])";
                }
            }
            else
            {
                return $"{className}({Operator}, {Operands})";
            }
        }
    }

    public class EquityQuery : QueryBase
    {
        public EquityQuery(string op, List<object> operands) : base(op, operands) { }

        public override Dictionary<string, List<string>> ValidFields => Consts.EQUITY_SCREENER_FIELDS;
        public override Dictionary<string, object> ValidValues => Consts.EQUITY_SCREENER_EQ_MAP;
    }

    public class FundQuery : QueryBase
    {
        public FundQuery(string op, List<object> operands) : base(op, operands) { }

        public override Dictionary<string, List<string>> ValidFields => Consts.FUND_SCREENER_FIELDS;
        public override Dictionary<string, object> ValidValues => Consts.FUND_SCREENER_EQ_MAP;
    }
}