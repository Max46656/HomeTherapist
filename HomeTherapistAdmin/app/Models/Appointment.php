<?php

namespace App\Models;

use Backpack\CRUD\app\Models\Traits\CrudTrait;
use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Relations\BelongsTo;
use Illuminate\Database\Eloquent\Relations\HasOne;

class Appointment extends Model
{
    use CrudTrait;
    use HasFactory;

    protected $fillable = [
        "user_id",
        'is_complete',
        'longitude',
        'latitude',
        'age_group',
        'gender',
        'customer_address',
        'customer_phone',
        'customer_ID',
        'start_dt',
    ];

    public function user(): BelongsTo
    {
        return $this->belongsTo(User::class, 'user_id', 'staff_id');
    }
    public function appointment_detail(): HasOne
    {
        return $this->hasOne(AppointmentDetail::class);
    }
}
