<?php

namespace Database\Seeders;

// use Illuminate\Database\Console\Seeds\WithoutModelEvents;

use App\Models\Appointment;
use App\Models\Calendar;
use App\Models\Order;
use App\Models\Service;
use App\Models\User;
use Database\Seeders\AdminSeeder;
use Database\Seeders\AppointmentDetailSeeder;
use Database\Seeders\AppointmentSeeder;
use Database\Seeders\ArticleSeeder;
use Database\Seeders\FeedbackSeeder;
use Database\Seeders\OrderDetailSeeder;
use Database\Seeders\OrderSeeder;
use Database\Seeders\ServiceSeeder;
use Database\Seeders\TherapistOpenServicesSeeder;
use Database\Seeders\TherapistOpenTimeSeeder;
use Database\Seeders\TruncateAll;
use Database\Seeders\UserSeeder;
use Illuminate\Database\Seeder;

class DatabaseSeeder extends Seeder
{

    /**
     * Seed the application's database.
     */
    public function run(): void
    {
        $this->call(TruncateAll::class);

        $calendars = Calendar::where('dt', '>=', now())
            ->whereTime('dt', '>=', '08:00:00')
            ->whereTime('dt', '<=', '20:00:00')
            ->get()
            ->pluck('dt')
            ->toArray();
        config(['seeding.calendars' => $calendars]);

        $this->call(UserSeeder::class);
        $usersID = User::all()->pluck('staff_id')->toArray();
        config(['seeding.users' => $usersID]);
        $this->call(ServiceSeeder::class);
        $services = collect(Service::all()->toArray());
        config(['seeding.services' => $services]);
        $this->call(TherapistOpenServicesSeeder::class);
        $this->call(TherapistOpenTimeSeeder::class);
        $this->call(AppointmentSeeder::class);
        $appointments = collect(Appointment::all()->toArray());
        config(['seeding.appointments' => $appointments]);
        $this->call(AppointmentDetailSeeder::class);
        $this->call(OrderSeeder::class);
        $orders = collect(Order::all()->toArray());
        config(['seeding.orders' => $orders]);
        $this->call(OrderDetailSeeder::class);
        $this->call(ArticleSeeder::class);
        $this->call(FeedbackSeeder::class);
        $this->call(AdminSeeder::class);
    }
}
