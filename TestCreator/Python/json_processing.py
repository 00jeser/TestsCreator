import json
from random import randint
from random import choice
from math import sqrt, sin, cos, tan, pi, acos, asin, atan


def proc_func(inp):  # Function for selecting random problems and numbers under certain conditions
    inp_changed = inp
    if not inp_changed['task']:
        rand_num = randint(1, len(inp_changed['tasks'])) - 1
        inp_changed['tasks'] = inp_changed['tasks'][rand_num]
        inp_changed['answers'] = inp_changed['answers'][rand_num]
        return inp_changed
    for i in range(len(inp_changed['variables'])):
        if inp_changed['variables'][i]['range']:
            perem = randint(int(inp_changed['variables'][i]['range'].split('/')[0]),
                            int(inp_changed['variables'][i]['range'].split('/')[1]))
        else:
            perem = choice(inp_changed['variables'][i]['choice'])
        inp_changed['task'] = inp_changed['task'].replace('{' + inp_changed['variables'][i]['name'] + '}', str(perem))
        if inp_changed['equation']:
            inp_changed['equation'] = inp_changed['equation'].replace(inp_changed['variables'][i]['name'], str(
                perem))  # Replacement variables in the 'equation'
    return inp_changed


def decision(inp):  # Problem solution
    if inp['equation']:
        if 'vector{' not in inp['equation']:
            inp['equation'] = inp['equation'].replace(':', '/')
            inp['equation'] = inp['equation'].replace('^', '**')
            inp['equation'] = inp['equation'].replace('√', 'sqrt')
            inp['equation'] = inp['equation'].replace('arccos', 'acos')
            inp['equation'] = inp['equation'].replace('arcsin', 'asin')
            inp['equation'] = inp['equation'].replace('arctan', 'atan')
            inp['equation'] = inp['equation'].split(';')
            for i in range(len(inp['equation'])):
                inp['equation'][i] = eval(inp['equation'][i].split('=')[1])
            return *inp['equation']
    if inp['equation'] and 'vector{' in inp['equation']:
        inp['equation'] = inp['equation'].split('=')[1]
        inp['equation'] = inp['equation'].replace('vector{', '')
        inp['equation'] = inp['equation'][:-1]
        return '{' + str(eval(inp['equation'].split(';')[0])) + '; ' + str(
            eval(inp['equation'].split(';')[1])) + '; ' + str(eval(inp['equation'].split(';')[2])) + '}'
    else:
        return inp['answers']


def main_func(input_json, number_of_variants):  # Main function for calling
    list_of_varianst = list()
    for variant in range(number_of_variants):
        data = json.loads(input_json)
        list_of_varianst.append([])
        for i in range(len(data)):
            data_ = proc_func(data[i])  # calling the proc_func(inp)
            data_['equation'] = decision(data_)  # calling the decision(inp)
            if not data_['task']:
                list_of_varianst[variant].append([data_['tasks'], data_['equation']])
            else:
                list_of_varianst[variant].append([data_['task'], data_['equation']])

    # ------- Output block ------- #
    list_of_output = list()
    for i_0 in range(len(list_of_varianst)):
        list_of_output.append(list())
        for i_1 in range(len(list_of_varianst[i_0])):
            list_of_output[i_0].append(dict())
            list_of_output[i_0][i_1]['task'] = list_of_varianst[i_0][i_1][0]  # Adding a response to input
            list_of_output[i_0][i_1]['answer'] = list_of_varianst[i_0][i_1][1]  # Adding a response to output
    list_of_output = json.dumps(list_of_output)
    return list_of_output
    # ------- Output block ------- #