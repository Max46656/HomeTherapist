<?php

namespace App\Models;

use Backpack\CRUD\app\Models\Traits\CrudTrait;
use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Relations\BelongsToMany;
use Illuminate\Database\Eloquent\Relations\HasMany;

class Calendar extends Model
{
    use CrudTrait;
    use HasFactory;

    protected $table = 'calendar';

    public function appointment(): HasMany
    {
        return $this->hasMany(Appointment::class, 'start_dt', 'dt');
    }

    /**
     * Get all of the order for the User
     *
     * @return \Illuminate\Database\Eloquent\Relations\HasMany
     */
    public function order(): HasMany
    {
        return $this->hasMany(Order::class, 'start_dt', 'dt');
    }

    /**
     * Get all of the open User for the time
     *
     * @return \Illuminate\Database\Eloquent\Relations\BelongsToMany
     */
    public function therapists()
    {
        return $this->belongsToMany(User::class, 'therapist_open_times', 'start_dt', 'user_id')
            ->using(TherapistOpenTime::class);
    }
}
