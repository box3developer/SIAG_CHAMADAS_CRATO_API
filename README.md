# GRENDENE CHAMADAS SIAG CRATO API

## Integrações
- [ ] Nodered: http://gra-lxsobcaracol.sob.ad-grendene.com:1880/

## Procedures
- [ ] sp_get_checklist_equipamento.sql
- [ ] sp_rotina_retornastageinlivre.sql
- [ ] sp_siag_alocacaoautomaticaoblaterais.sql
- [ ] sp_siag_atualizaequipamento.sql
- [ ] sp_siag_busca_qtde_pallets.sql
- [ ] sp_siag_buscaarealivre.sql
- [ ] sp_siag_criachamada.sql
- [ ] sp_siag_destinopallet.sql
- [ ] sp_siag_finalizachamada.sql
- [ ] sp_siag_loginoperador.sql
- [ ] sp_siag_logoffoperador.sql
- [ ] sp_siag_performanceonline.sql
- [ ] sp_siag_reiniciachamada.sql
- [ ] sp_siag_rejeitachamada.sql
- [ ] sp_siag_selecionachamada.sql

## Definições de atividades
|Atividade                                     | Atividade Tarefa
|----------------------------------------------|-----------------------------------
| Pallet cheio no sorter                       | Pegue o pallet cheio
| Colocar pallet vazio no sorter               | Repor pallet no sorter
| Levar o pallet cheio para o buffer           | Deixe o pallet cheio no buffer
| Carga inicial de pallet                      | Leve um pallet vazio para a posição
| Buscar pallet no buffer                      | Pegar pallet no buffer
|                                              | Escolhe um endereço de destino para o pallet
|                                              | Reserva a área de destino (DESATIVADO)
|                                              | Desaloca o pallet (DESATIVADO)
|                                              | Leve o pallet até a área de leitura
|                                              | Reserva posição no stage-in (DESATIVADO)
| Levar pallet do buffer para stage-in         | Levar pallet para o stage-in
|                                              | Aloca o pallet no stage-in (DESATIVADO)
|                                              | Redefine a área de origem da chamada (DESATIVADO)
| Armazenar no porta-pallet                    | Armazenar o pallet
| Retirar pallet para expedição                | Define uma posição de stage-out
| Buscar pallet no stage-out                   | Buscar o pallet no stage-out
|                                              | Redefine pallet da chamada (DESATIVADO/26)
|                                              | Desaloca o stage-out
|                                              | Levar pallet para doca (DESATIVADO)
| Recolher pallets vazios                      | Recolher os pallets vazios
| Mover pallet para stage-out                  | Retirar o pallet
|                                              | Redefine o destino na expedição
| Levar pallet do stage-out para a doca        | Levar pallet para doca
| Abastecer a doca com um pallet vazio         | Abastecer a doca com um pallet vazio
| Retirar pallet para movimentação interna     | Define uma posição de stage-out
| Mover pallet para stage-out - Ordem interna  | Retirar o pallet
|                                              | Redefine o destino na expedição
| Buscar pallet no stage-out - Ordem interna   | Buscar o pallet no stage-out
|                                              | Redefine pallet da chamada (DESATIVADO/26)
|                                              | Desaloca o stage-out
| Levar pallet do stage-out para ordem interna | Levar pallet para doca
| Retirar pallet para transferência            | Define uma posição de stage-out
| Mover pallet para stage-out - Transferência  | Retirar o pallet
|                                              | Redefine o destino na expedição
| Buscar pallet no stage-out - Transferência   | Buscar o pallet no stage-out
|                                              | Desaloca o stage-out
| Levar pallet do stage-out para buffer        | Levar pallet para buffer

## Tabelas
1. areaarmazenagem
2. atividade
3. atividaderejeicao
4. atividaderotina
5. atividadetarefa
6. chamada
7. chamadatarefa
8. endereco
9. equipamento
10. equipamentochecklistoperador
11. operador
12. pallet
13. setortrabalho

## Tabelas sem Model definido (uso diretamente em queries)
14. tmp_transicaochamada
15. logsiag
