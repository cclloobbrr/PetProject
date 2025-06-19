using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Test_task.Models.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Test_task.Data.Repositories
{
    public class ChecksRepository
    {
        private readonly Test_taskDbContext _dbContext;

        public ChecksRepository(Test_taskDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<CheckEntity>> GetAll()
        {
            return await _dbContext.Checks
                .AsNoTracking()
                .Include(c => c.SMP)
                .Include(c => c.Supervisory)
                .ToListAsync();
        }

        public async Task Delete(Guid id)
        {
            var check = await _dbContext.Checks
                .FirstOrDefaultAsync(c => c.Id == id);
            if(check != null)
            {
                _dbContext.Checks.Remove(check);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<List<CheckEntity>> Search(string name = null, DateOnly? date = null)
        {

            var checks = _dbContext.Checks
                .AsNoTracking()
                .Include(c => c.SMP)
                .Include(c => c.Supervisory)
                .AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                checks = checks.Where(c => c.SMP.Name.Contains(name));
            }
            if (date != null)
            {
                checks = checks.Where(c => c.DateStart == date);
            }
            return await checks.ToListAsync();
        }

        public async Task AddSMP(Guid id, string name)
        {
            var _SMP = new SMPEntity
            {
                Id = id,
                Name = name
            };
            await _dbContext.AddAsync(_SMP);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<SupervisoryEntity>> GetSupervisoryList()
        {
            return await _dbContext.Supervisory
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<CheckEntity> GetById(Guid id)
        {
            return await _dbContext.Checks
                .AsNoTracking()
                .Include(c => c.SMP)
                .Include(c => c.Supervisory)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<SMPEntity> GetSMP(string name)
        {
            return await _dbContext.SMP
                .AsNoTracking()
                .FirstOrDefaultAsync(sm => sm.Name == name);
        }

        public async Task UpdateCheck(CheckEntity check)
        {
            _dbContext.Checks.Update(check);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Add(Guid id, Guid _SMPId, Guid supervisoryId, DateOnly dateStart, DateOnly dateFinish, int plannedDuration)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var checkEntity = new CheckEntity
                {
                    Id = id,
                    SMPId = _SMPId,
                    SupervisoryId = supervisoryId,
                    DateStart = dateStart,
                    DateFinish = dateFinish,
                    PlannedDuration = plannedDuration
                };

                await _dbContext.AddAsync(checkEntity);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<byte[]> ExportChecksToExcel(List<CheckEntity> checks)
        {
            if (checks == null)
            {
                throw new ArgumentNullException(nameof(checks));
            }

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Реестр плановых проверок");

            worksheet.Cell(1, 1).Value = "СМП";
            worksheet.Cell(1, 2).Value = "Контролирующий орган";
            worksheet.Cell(1, 3).Value = "Дата начала проверки";
            worksheet.Cell(1, 4).Value = "Дата окончания проверки";
            worksheet.Cell(1, 5).Value = "Плановая длительность проверки (дни)";


            int row = 2;
            foreach(var check in checks)
            {
                worksheet.Cell(row, 1).Value = check.SMP.Name;
                worksheet.Cell(row, 2).Value = check.Supervisory.Name;
                worksheet.Cell(row, 3).Value = check.DateStart.ToString("dd.MM.yyyy");
                worksheet.Cell(row, 4).Value = check.DateFinish.ToString("dd.MM.yyyy");
                worksheet.Cell(row, 5).Value = check.PlannedDuration;
                row++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public async Task<(int added, int alreadyThere, int errors)> ImportChecksFromExcel(Stream fileStream)
        {
            using var workbook = new XLWorkbook(fileStream);
            var worksheet = workbook.Worksheet(1);

            int addedCount = 0;
            int alreadyThereCount = 0;
            int errorsCount = 0;

            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                foreach (var row in worksheet.RowsUsed().Skip(1))
                {
                    try
                    {
                        var smpName = row.Cell(1).GetString();

                        var smp = await GetSMP(smpName);
                        if(smp == null)
                        {
                            await AddSMP(Guid.NewGuid(), smpName);
                            smp = await GetSMP(smpName);
                        }

                        var supervisoryName = row.Cell(2).GetString();

                        var supervisory = await _dbContext.Supervisory.FirstOrDefaultAsync(s => s.Name == supervisoryName);
                        if (supervisory == null)
                        {
                            supervisory = new SupervisoryEntity { Id = Guid.NewGuid(), Name = supervisoryName };
                            await _dbContext.Supervisory.AddAsync(supervisory);
                        }

                        var dateStart = DateOnly.Parse(row.Cell(3).GetString());
                        var plannedDuration = Convert.ToInt32(row.Cell(5).GetString());
                        var dateFinish = dateStart.AddDays(plannedDuration);

                        var check = await _dbContext.Checks
                            .AsNoTracking()
                            .FirstOrDefaultAsync(c => c.SMPId == smp.Id
                                                    && c.SupervisoryId == supervisory.Id
                                                    && c.DateStart == dateStart
                                                    && c.DateFinish == dateFinish
                                                    && c.PlannedDuration == plannedDuration);
                        if(check != null)
                        {
                            throw new InvalidOperationException("Такая запись уже существует");
                        }

                        var newCheck = new CheckEntity
                        {
                            Id = Guid.NewGuid(),
                            SMPId = smp.Id,
                            SupervisoryId = supervisory.Id,
                            DateStart = dateStart,
                            DateFinish = dateFinish,
                            PlannedDuration = plannedDuration
                        };

                        await _dbContext.Checks.AddAsync(newCheck);
                        addedCount++;
                    }
                    catch(InvalidOperationException ex)
                    {
                        alreadyThereCount++;
                        Console.WriteLine(ex.Message);
                    }
                    catch
                    {
                        errorsCount++;
                        Console.WriteLine("Ошибка добавления");
                    }
                }
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

            return (addedCount, alreadyThereCount, errorsCount);            
        }
    }
}
