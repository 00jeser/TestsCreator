import json
from random import randint
from random import choice

number_of_variants = int(input())
list_of_varianst = list()


def main_func(inp):
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


for variant in range(number_of_variants):
    with open('txt.json', 'r', encoding='utf-8') as file:
        data = json.load(file)
    list_of_varianst.append(['Вариант ' + str(variant + 1) + ':'])
    for i in range(len(data)):
        data_ = main_func(data[i])
        if not data_['task']:
            list_of_varianst[variant].append('Задача ' + str(i + 1) + ': ' + data_['tasks'])
        else:
            list_of_varianst[variant].append('Задача ' + str(i + 1) + ': ' + data_['task'])

# ------- Блок вывода ------- #
print('-' * 100)
for i in range(len(list_of_varianst)):
    print(*list_of_varianst[i], sep='\n')
    print('-' * 100)
# ------- Блок вывода ------- #
