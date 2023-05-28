namespace SpaceBattle.Entities.Builders;

using System.Reflection;
using System.Text;
using Scriban;
using SpaceBattle.Base;

public class AdapterSourceCodeBuilder : IBuilder
{

    private IEnumerable<PropertyInfo> propeties;
    private IEnumerable<MethodInfo> methods;
    private Type dtype;

    //     private Template template = Template.Parse(
    //         text: @"
    //     internal class {{ class_name }} : {{ int_name }} {
    //         private IDictionary<string, object> data;

    //         public {{ class_name }}(IDictionary<string, object> data){
    //             this.data = data;
    //         }

    //         {{ for property in properties }}
    //         {{ full_property_name = property.PropertyType.Name + ""."" + property.Name }}
    //         public {{ property.PropertyType.Name }} {{ property.Name }} {
    //             {{ if property.CanRead }}get => ({{ property.PropertyType.Name }})data[""{{ full_property_name }}""];{{ end }}
    //             {{ if property.CanWrite }}set => data[""{{ full_property_name }}""] = value;{{ end }}
    //         }
    //         {{ end }}

    //         {{ for method in methods }}
    //         {{ full_method_name = method.ReturnType.Name + ""."" + method.Name }} // Other signatures?
    //         {{ params = method.GetParameters() }}
    //         public {{ method.ReturnType.Name }} {{ method.Name }}({{ for param in params  }}{{ param.ParameterType.Name }} {{ param.Name }}, {{ end }}) {
    //             {{ if method.ReturnType.Name != ""void"" }}
    //             return ({{ method.ReturnType.Name }})data[""{{ full_method_name }}""]({{ for param in params  }}{{ param.Name }}, {{ end }});
    //             {{ else }}
    //             data[""{{ full_method_name }}""]({{ for param in params  }}{{ param.Name }}, {{ end }});
    //             {{ end }}
    //         }
    //         {{ end }}
    //     }
    // ");
    private Template template = Template.Parse(text:
@"{{- func join(seq,del)
    result = """"
    if seq.size > 0
        for $i in (0..<(seq.size - 1))
            result = result + seq[$i] + del
        end
        result = result + seq[seq.size - 1]
    end
    ret result
    end
}}
{{- func map(seq,f)
    result = []
    i = 0
    for val in seq
        result[i] = f val
        i = i + 1
    end
    ret result
    end
}}
internal class {{ class_name }} : {{ int_name }} {
    private System.Collections.Generic.IDictionary<string, object> data;

    public {{ class_name }}(System.Collections.Generic.IDictionary<string, object> data) {
        this.data = data;
    }
    {{- for property in properties }}
    {{ full_property_name = property.type + ""."" + property.name }}
    public {{ property.type }} {{ property.name }} {
        {{
            if property.can_read
                ""get => ("" + property.type + "")data[\"""" + full_property_name + ""\""];""
            end
            if property.can_write
                ""set => data[\"""" + full_property_name + ""\""] = value;""
            end
        }}
    }
    {{- end }}
    {{- for method in methods }}
    {{ full_method_name = method.type + ""."" + method.name }}
    public {{ method.type }} {{ method.name }} ({{ method.parameters | map @(do; ret $0.type + "" "" + $0.name; end) | join "", "" }}) {
        {{
            if method.type != ""void""
                ""return ((System.Func<"" + (method.parameters | map @(do; ret $0.type; end) | join "","") + method.type + "">)""
            else
                ""((System.Action<"" + (method.parameters | map @(do; ret $0.type; end) | join "","") + "">)""
            end
        -}}
        data[""{{ full_method_name }}""])({{ method.parameters | map @(do; ret $0.name; end) | join "", ""}});
    }
    {{- end }}
}");

    private readonly IDictionary<string, Action<object[]>> configHandlers = new Dictionary<string, Action<object[]>>{
        {"Property",
            argv => {
                var ctx = (AdapterSourceCodeBuilder)argv[0];
                var propInfo = (PropertyInfo)argv[1];
                ctx.propeties = ctx.propeties.Append(propInfo).ToArray();
            }
        },
        {"Method",
            argv => {
                var ctx = (AdapterSourceCodeBuilder)argv[0];
                var methodInfo = (MethodInfo)argv[1];
                ctx.methods = ctx.methods.Append(methodInfo).ToArray();
            }
        },
        {"Dtype",
            argv => {
                var ctx = (AdapterSourceCodeBuilder)argv[0];
                ctx.dtype = (Type)argv[1];
            }
        }
        };

    public AdapterSourceCodeBuilder()
    {
        this.methods = new LinkedList<MethodInfo>();
        this.propeties = new LinkedList<PropertyInfo>();
        this.dtype = null!;
    }

    // Create adapters like object with dict field
    public IBuilder Config(string param, params object[] argv) // Params? maybe 1
    {
        Action<object[]> handler = configHandlers[param];
        handler(new object[] { this }.Concat(argv).ToArray());
        return this;
    }

    public object GetOrCreate()
    {
        object model = new
        {
            class_name = dtype.Name + "_adapter",
            int_name = dtype.FullName,
            properties = this.propeties.Select(
                (PropertyInfo p) =>
                {
                    object property = new
                    {
                        name = p.Name,
                        type = p.PropertyType.FullName,
                        can_read = p.CanRead,
                        can_write = p.CanWrite
                    };
                    return property;
                }
            ).ToList(),
            methods = this.methods.Select(
                (MethodInfo m) =>
                {
                    object method = new
                    {
                        name = m.Name,
                        type = m.ReturnType.FullName == "System.Void" ? "void" : m.ReturnType.FullName,
                        parameters = m.GetParameters().Select(
                            (ParameterInfo p) =>
                            {
                                var param = new
                                {
                                    name = p.Name,
                                    type = p.ParameterType.FullName
                                };
                                return param;
                            }
                        ).ToList()
                    };
                    return method;
                }
            ).ToList()
        };

        return this.template.Render(model);
    }
}
