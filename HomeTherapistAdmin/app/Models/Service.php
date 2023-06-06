<?php

namespace App\Models;

use App\Models\User;
use Backpack\CRUD\app\Models\Traits\CrudTrait;
use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Relations\BelongsToMany;
use Illuminate\Database\Eloquent\Relations\HasMany;

class Service extends Model
{
    use CrudTrait;
    use HasFactory;

    public function users()
    {
        return $this->belongsToMany(User::class, 'therapist_open_services', 'services_id', 'user_id')
            ->using(TherapistOpenService::class);
    }
    public function order_detail(): HasMany
    {
        return $this->hasMany(OrderDetail::class);
    }
    public function appointment_detail(): HasMany
    {
        return $this->hasMany(AppointmentDetail::class);
    }
}
