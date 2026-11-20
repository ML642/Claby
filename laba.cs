using System;
using System.Collections.Generic;
using System.Linq;

public static class Herman
{
    // 1. Duże, niedostarczone przesyłki według wagi malejąco.
    public static IEnumerable<(string Code, decimal Weight, DateTime CreatedAt)>
    GetLargeUndelivered(DistributionCenter center)
    {
        return center.Shipments.Values
            .Where(s => s.Parcel.Size == ParcelSize.Large &&
                        s.Status != ShipmentStatus.Delivered)
            .OrderByDescending(s => s.Parcel.WeightKg)
            .Select(s => (
                s.TrackingCode,
                s.Parcel.WeightKg,
                s.CreatedAt
            ));
    }

    // 2. Kody przesyłek od najstarszej do najnowszej.
    public static IEnumerable<string> CodesByDate(DistributionCenter c) =>
        c.Shipments.Values.OrderBy(s => s.CreatedAt).Select(s => s.TrackingCode);

    // 3. Przesyłki cięższe od podanej wagi.
    public static IEnumerable<ShipmentOrder> HeavierThan(DistributionCenter c, decimal kg) =>
        c.Shipments.Values.Where(s => s.Parcel.WeightKg > kg);

    // 4. Liczba przesyłek w każdym statusie.
    public static IEnumerable<(ShipmentStatus Status, int Count)> CountByStatus(DistributionCenter c) =>
        c.Shipments.Values.GroupBy(s => s.Status).Select(g => (g.Key, g.Count()));

    // 5. Łączna waga przesyłek w każdym rozmiarze.
    public static IEnumerable<(ParcelSize Size, decimal Kg)> WeightBySize(DistributionCenter c) =>
        c.Shipments.Values.GroupBy(s => s.Parcel.Size)
            .Select(g => (g.Key, g.Sum(s => s.Parcel.WeightKg)));

    // 6. Przesyłki z imieniem nadawcy: SenderId -> Id.
    public static IEnumerable<(string Code, string Name)> WithSender(DistributionCenter c) =>
        c.Shipments.Values.Join(c.Senders.Values, s => s.SenderId, p => p.Id,
            (s, p) => (s.TrackingCode, p.Name));

    // 7. Przesyłki z imieniem odbiorcy: RecipientId -> Id.
    public static IEnumerable<(string Code, string Name)> WithRecipient(DistributionCenter c) =>
        c.Shipments.Values.Join(c.Recipients.Values, s => s.RecipientId, p => p.Id,
            (s, p) => (s.TrackingCode, p.Name));

    // 8. Odbiorcy bez przesyłek: GroupJoin zachowuje także osoby bez dopasowania.
    public static IEnumerable<Recipient> RecipientsWithoutShipments(DistributionCenter c) =>
        c.Recipients.Values.GroupJoin(c.Shipments.Values, p => p.Id, s => s.RecipientId,
            (p, shipments) => new { Person = p, Shipments = shipments })
            .Where(x => !x.Shipments.Any()).Select(x => x.Person);

    // 9. Liczba przesyłek każdego nadawcy, łącznie z zerami.
    public static IEnumerable<(string Name, int Count)> CountBySender(DistributionCenter c) =>
        c.Senders.Values.GroupJoin(c.Shipments.Values, p => p.Id, s => s.SenderId,
            (p, shipments) => (p.Name, shipments.Count()));

    // 10. Trzech nadawców z największą liczbą przesyłek.
    public static IEnumerable<(string Name, int Count)> TopSenders(DistributionCenter c) =>
        CountBySender(c).OrderByDescending(x => x.Count).Take(3);

    // 11. Przesyłki wybranego nadawcy.
    public static IEnumerable<ShipmentOrder> BySender(DistributionCenter c, string id) =>
        c.Shipments.Values.Where(s => s.SenderId == id);

    // 12. Przesyłki utworzone po dacie.
    public static IEnumerable<ShipmentOrder> CreatedAfter(DistributionCenter c, DateTime date) =>
        c.Shipments.Values.Where(s => s.CreatedAt > date);

    // 13. Przesyłki z nazwą paczkomatu źródłowego (opcjonalny FK).
    public static IEnumerable<(string Code, string Locker)> WithSourceLocker(DistributionCenter c) =>
        c.Shipments.Values.Where(s => s.SourceLockerId != null)
            .Join(c.Lockers.Values, s => s.SourceLockerId, l => l.Id,
                (s, l) => (s.TrackingCode, l.Name));

    // 14. Przesyłki z nazwą paczkomatu docelowego (opcjonalny FK).
    public static IEnumerable<(string Code, string Locker)> WithDestinationLocker(DistributionCenter c) =>
        c.Shipments.Values.Where(s => s.DestinationLockerId != null)
            .Join(c.Lockers.Values, s => s.DestinationLockerId, l => l.Id,
                (s, l) => (s.TrackingCode, l.Name));

    // 15. Przesyłki bez kuriera dostarczającego.
    public static IEnumerable<ShipmentOrder> WithoutCourier(DistributionCenter c) =>
        c.Shipments.Values.Where(s => s.DeliveryCourierId == null);

    // 16. Przesyłki z nazwą kuriera dostarczającego.
    public static IEnumerable<(string Code, string Courier)> WithCourier(DistributionCenter c) =>
        c.Shipments.Values.Where(s => s.DeliveryCourierId != null)
            .Join(c.Couriers.Values, s => s.DeliveryCourierId, p => p.Id,
                (s, p) => (s.TrackingCode, p.Name));

    // 17. Liczba dostaw każdego kuriera, łącznie z zerami.
    public static IEnumerable<(string Name, int Count)> CountByCourier(DistributionCenter c) =>
        c.Couriers.Values.GroupJoin(c.Shipments.Values, p => p.Id, s => s.DeliveryCourierId,
            (p, shipments) => (p.Name, shipments.Count()));

    // 18. Wszystkie zdarzenia śledzenia: SelectMany rozwija listę zdarzeń na wiersze.
    public static IEnumerable<(string Code, DateTime Time, ShipmentStatus Status)> AllEvents(DistributionCenter c) =>
        c.TrackingHistory.SelectMany(pair => pair.Value,
            (pair, e) => (pair.Key, e.Timestamp, e.Status));

    // 19. Ostatnie zdarzenie dla każdej przesyłki z historią.
    public static IEnumerable<(string Code, DateTime Time, ShipmentStatus Status)> LastEvents(DistributionCenter c) =>
        c.TrackingHistory.Where(pair => pair.Value.Any())
            .Select(pair => new { pair.Key, Event = pair.Value.OrderByDescending(e => e.Timestamp).First() })
            .Select(x => (x.Key, x.Event.Timestamp, x.Event.Status));

    // 20. Przesyłki bez żadnego zdarzenia śledzenia.
    public static IEnumerable<ShipmentOrder> WithoutEvents(DistributionCenter c) =>
        c.Shipments.Values.Where(s => !c.TrackingHistory.ContainsKey(s.TrackingCode) ||
            !c.TrackingHistory[s.TrackingCode].Any());

    // 21. Показать код посылки, имя отправителя и имя получателя.
    // Два Join проходят по двум внешним ключам одной посылки.
    public static IEnumerable<(string Code, string Sender, string Recipient)> SenderToRecipient(DistributionCenter c) =>
        c.Shipments.Values
            .Join(c.Senders.Values, s => s.SenderId, p => p.Id,
                (s, p) => new { Shipment = s, Sender = p })
            .Join(c.Recipients.Values, x => x.Shipment.RecipientId, p => p.Id,
                (x, p) => (x.Shipment.TrackingCode, x.Sender.Name, p.Name));

    // 22. Найти пары отправитель-получатель с несколькими посылками.
    // Группируем по составному ключу, затем соединяем с двумя справочниками.
    public static IEnumerable<(string Sender, string Recipient, int Count)> RepeatedRoutes(DistributionCenter c) =>
        c.Shipments.Values
            .GroupBy(s => new { s.SenderId, s.RecipientId })
            .Where(g => g.Count() > 1)
            .Join(c.Senders.Values, g => g.Key.SenderId, p => p.Id,
                (g, p) => new { Group = g, Sender = p })
            .Join(c.Recipients.Values, x => x.Group.Key.RecipientId, p => p.Id,
                (x, p) => (x.Sender.Name, p.Name, x.Group.Count()))
            .OrderByDescending(x => x.Item3);

    // 23. Средний вес посылки для каждой пары «город назначения + размер».
    public static IEnumerable<(string City, ParcelSize Size, decimal AverageKg)> AverageWeightByCityAndSize(DistributionCenter c) =>
        c.Shipments.Values
            .GroupBy(s => new { s.DestinationCity, s.Parcel.Size })
            .Select(g => (g.Key.DestinationCity, g.Key.Size,
                g.Average(s => s.Parcel.WeightKg)))
            .OrderBy(x => x.Item1).ThenBy(x => x.Item2);

    // 24. Пять доставленных посылок, которые шли дольше всех.
    // Nullable DeliveredAt проверяется до обращения к Value.
    public static IEnumerable<(string Code, double Days)> LongestDeliveries(DistributionCenter c) =>
        c.Shipments.Values
            .Where(s => s.DeliveredAt.HasValue)
            .Select(s => new
            {
                Code = s.TrackingCode,
                Days = (s.DeliveredAt.Value - s.CreatedAt).TotalDays
            })
            .OrderByDescending(x => x.Days)
            .Take(5)
            .Select(x => (x.Code, x.Days));

    // 25. Просроченные на указанную дату посылки, которые ещё не доставлены.
    public static IEnumerable<ShipmentOrder> OverdueUndelivered(DistributionCenter c, DateTime now) =>
        c.Shipments.Values
            .Where(s => s.PlannedDeliveryDate.HasValue &&
                        s.PlannedDeliveryDate.Value < now &&
                        s.Status != ShipmentStatus.Delivered)
            .OrderBy(s => s.PlannedDeliveryDate);

    // 26. Найти расхождения между текущим статусом и последним событием истории.
    public static IEnumerable<(string Code, ShipmentStatus Current, ShipmentStatus LastEvent)> StatusMismatches(DistributionCenter c) =>
        c.Shipments.Values
            .Join(c.TrackingHistory, s => s.TrackingCode, pair => pair.Key,
                (s, pair) => new { Shipment = s, Events = pair.Value })
            .Where(x => x.Events.Any())
            .Select(x => new
            {
                Code = x.Shipment.TrackingCode,
                Current = x.Shipment.Status,
                LastEvent = x.Events.OrderByDescending(e => e.Timestamp).First().Status
            })
            .Where(x => x.Current != x.LastEvent)
            .Select(x => (x.Code, x.Current, x.LastEvent));

    // 27. Для каждого пункта назначения найти число входящих посылок и общий вес.
    // GroupJoin сохраняет пункты, в которые пока ничего не отправлено.
    public static IEnumerable<(string Locker, int Count, decimal WeightKg)> IncomingByLocker(DistributionCenter c) =>
        c.Lockers.Values
            .GroupJoin(c.Shipments.Values, l => l.Id, s => s.DestinationLockerId,
                (l, shipments) => new { Locker = l, Shipments = shipments })
            .Select(x => (x.Locker.Name, x.Shipments.Count(),
                x.Shipments.Sum(s => s.Parcel.WeightKg)))
            .OrderByDescending(x => x.Item2);

    // 28. Посылки, у которых курьер указан в FK, но код отсутствует в его списке.
    public static IEnumerable<(string Code, string Courier)> CourierAssignmentMismatches(DistributionCenter c) =>
        c.Shipments.Values
            .Where(s => s.DeliveryCourierId != null)
            .Join(c.Couriers.Values, s => s.DeliveryCourierId, courier => courier.Id,
                (s, courier) => new { Shipment = s, Courier = courier })
            .Where(x => !x.Courier.AssignedTrackingCodes.Contains(x.Shipment.TrackingCode))
            .Select(x => (x.Shipment.TrackingCode, x.Courier.Name));

    // 29. Очередь ожидающих назначения: позиция, код и текущий статус.
    // Select с индексом сохраняет порядок очереди, Join находит объект посылки.
    public static IEnumerable<(int Position, string Code, ShipmentStatus Status)> AssignmentQueue(DistributionCenter c) =>
        c.DeliveriesAwaitingAssignment
            .Select((code, index) => new { Code = code, Position = index + 1 })
            .Join(c.Shipments.Values, x => x.Code, s => s.TrackingCode,
                (x, s) => (x.Position, x.Code, s.Status));

    // 30. Рейтинг курьеров по числу доставленных посылок и среднему времени доставки.
    // Курьеры без доставок остаются в ответе; среднее для них равно null.
    public static IEnumerable<(string Courier, int Delivered, double? AverageDays)> CourierDeliveryRanking(DistributionCenter c) =>
        c.Couriers.Values
            .GroupJoin(c.Shipments.Values.Where(s => s.DeliveredAt.HasValue),
                courier => courier.Id, s => s.DeliveryCourierId,
                (courier, shipments) => new
                {
                    courier.Name,
                    Delivered = shipments.Count(),
                    AverageDays = shipments.Any()
                        ? (double?)shipments.Average(s => (s.DeliveredAt.Value - s.CreatedAt).TotalDays)
                        : null
                })
            .OrderByDescending(x => x.Delivered)
            .ThenBy(x => x.AverageDays)
            .Select(x => (x.Name, x.Delivered, x.AverageDays));
}
