import json
from random import randint
from random import choice


def proc_func(inp):
    inp_changed = inp
    if not inp_changed['task']:
        inp_changed['tasks'] = choice(inp_changed['tasks'])
        return inp_changed
    for i in range(len(inp_changed['variables'])):
        if inp_changed['variables'][i]['range']:
            perem = randint(int(inp_changed['variables'][i]['range'].split('/')[0]),
                            int(inp_changed['variables'][i]['range'].split('/')[1]))
        else:
            perem = choice(inp_changed['variables'][i]['choice'])
        inp_changed['task'] = inp_changed['task'].replace('{' + inp_changed['variables'][i]['name'] + '}', str(perem))
    return inp_changed


def main_func(input_json, number_of_variants):
    list_of_varianst = list()
    for variant in range(number_of_variants):
        with open(input_json, 'r', encoding='utf-8') as file:
            data = json.load(file)
        list_of_varianst.append([])
        for i in range(len(data)):
            data_ = proc_func(data[i])
            if not data_['task']:
                list_of_varianst[variant].append(data_['tasks'])
            else:
                list_of_varianst[variant].append(data_['task'])

    # ------- Блок вывода ------- #
    list_of_output = list()
    for i_0 in range(len(list_of_varianst)):
        list_of_output.append(dict())
        for i_1 in range(len(list_of_varianst[i_0])):
            list_of_output[i_0]['task' + str(i_1 + 1)] = list_of_varianst[i_0][i_1]
    return list_of_output
    # ------- Блок вывода ------- #
