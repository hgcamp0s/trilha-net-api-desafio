using Microsoft.AspNetCore.Mvc;
using TrilhaApiDesafio.Context;
using TrilhaApiDesafio.Models;

namespace TrilhaApiDesafio.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TarefaController : ControllerBase
    {
        private readonly OrganizadorContext _context;

        public TarefaController(OrganizadorContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public IActionResult ObterPorId(int id)
        {
            var tarefa = _context.Tarefas.Find(id); // Busca o Id no banco utilizando o EF

            if (tarefa == null) // Valida o tipo de retorno, e se, não encontrar tarefa 'null'
                return NotFound(); // Retorna NotFound

            return Ok(tarefa); // Caso encontre, retorna OK com a tarefa encontrada
        }

        [HttpGet("ObterTodos")]
        public IActionResult ObterTodos()
        {
            var tarefa = _context.Tarefas.ToList(); // Lista todas as tarefas no banco utilizando o EF
            return Ok(tarefa);
        }

        [HttpGet("ObterPorTitulo")]
        public IActionResult ObterPorTitulo(string titulo)
        {
            var tarefa = _context.Tarefas.Where(x => x.Titulo.Contains(titulo)); // Busca as tarefas no banco utilizando o EF, que contenha o titulo recebido por parâmetro
            return Ok(tarefa);
        }

        [HttpGet("ObterPorData")]
        public IActionResult ObterPorData(DateTime data)
        {
            var tarefa = _context.Tarefas.Where(x => x.Data.Date == data.Date); // Busca as tarefas no banco, que contenha a data recebido por parâmetro
            return Ok(tarefa);
        }

        [HttpGet("ObterPorStatus")]
        public IActionResult ObterPorStatus(EnumStatusTarefa? status)
        {
            // Verifica se a opção fornecida pelo usuário será "Pendente" ou "Finalizado"
            if (status != EnumStatusTarefa.Pendente && status != EnumStatusTarefa.Finalizado)
            {
                return BadRequest(new { Erro = "Escolha uma das duas opções: Pendente ou Finalizado" }); // Se não for, retornará uma mensagem de erro
            }

            var tarefa = _context.Tarefas.Where(x => status == null || x.Status == status); // Busca as tarefas no banco, que contenha o status recebido por parâmetro

            return Ok(tarefa);
        }

        [HttpPost]
        public IActionResult Criar(Tarefa tarefa)
        {
            if (tarefa.Data == DateTime.MinValue)
                return BadRequest(new { Erro = "A data da tarefa não pode ser vazia" });

            _context.Add(tarefa); // Adiciona uma nova tarefa no banco utilizando EF
            _context.SaveChanges(); // Salva essas mudanças

            return CreatedAtAction(nameof(ObterPorId), new { id = tarefa.Id }, tarefa);
        }

        [HttpPut("{id}")]
        public IActionResult Atualizar(int id, Tarefa tarefa)
        {
            var tarefaBanco = _context.Tarefas.Find(id);

            if (tarefaBanco == null)
                return NotFound();

            if (tarefa.Data == DateTime.MinValue)
                return BadRequest(new { Erro = "A data da tarefa não pode ser vazia" });

            // Atualiza as informações da variável tarefaBanco com a tarefa recebida via parâmetro
            tarefaBanco.Titulo = tarefa.Titulo;
            tarefaBanco.Descricao = tarefa.Descricao;
            tarefaBanco.Data = tarefa.Data;
            tarefaBanco.Status = tarefa.Status;

            _context.Tarefas.Update(tarefaBanco); // Atualiza a variável tarefaBanco no EF
            _context.SaveChanges();

            return Ok(tarefaBanco);
        }

        [HttpDelete("{id}")]
        public IActionResult Deletar(int id)
        {
            var tarefaBanco = _context.Tarefas.Find(id);

            if (tarefaBanco == null)
                return NotFound();

            _context.Tarefas.Remove(tarefaBanco); // Remove a tarefa encontrada
            _context.SaveChanges();

            return NoContent();
        }
    }
}
